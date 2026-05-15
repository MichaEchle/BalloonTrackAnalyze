# Task constraints

## Context
The competitors have different tasks to absolve and will be ranged by their result. However most tasks have some constraints.
I want you to plan the constraint system.

## Constraints targets
Constraints can be applied to:
- position at declaration
- declared goal
- markers
- judge declared goals

## Types of constraints
Common constraints are (non exhaustive list):
- Distance between position at declaration and declared goal
- Height limits for declared goal altitude
- Distance between markers and declared goal
- Distance between markers
- Distance between judge declared goals and declared goal
- Timing constraints for marker drops (e.g. only between minute 10 and 20 of every hour)
- Declaring a goal before a certain time (e.g. before 10:00 am)
- Declaring a goal east, south, west or north of a grid line
- Timing constraints between markers (e.g. at least 5 minutes between marker drops)

## Constraint chains
Typically a task have multiple constraints which are all applied together. However some constraints are exclusive and only must be valid.
There must be some logic to chain constraints together with AND and OR operations.
For example: Distance between position at declaration and declared goal min 2km and max 5km and (markers drops at minute 5-10, 25-30, or 40-45 in every hour).

## Further complexities
- there can be multiple contraint targets (e.g. a pilot is allowed to declared a goal multiple times using the same goal number)
  - sometimes the first valid target must be chosen, sometime the last valid
- not all constraints are enforced the same way
  - sometimes each violation of a constraint causes the target to be invalid
  - sometimes a violation is penalized instead if invalidating the target
  - the evaulation differes, sometimes evaluations stops when a violation is found, othertimes the next target is evaluated	 
	
## Constraint goals
Constraints have multiple purposes:
- select valid targets to use for scoring the actual task (e.g. 4 goals have been declared, the constraint should must determine the lasted valid goal)
  - if no valid target is found, the task is not scored (no result)
- calculate penalties for infringements

## Design considerations and my struggles
- Based on the different types of constraints, I had difficulties defining a meaningful interface that each constraint can share
- The fundamental checks that must be performed are rather simple (distance checks, timing checks, etc.) and the challenge is more selecting the input for these checks and sharing the checks between different constraints (e.g. distance between position at declaration and declared goal, distance between markers and declared goal, distance between markers, etc. all require distance checks but with different inputs)
- The chaining of constraints is also a challenge, as it can be complex and I want to avoid hardcoding specific chains in the code
- Creating a API that supports the flexibility but is reasonable to use, you want to define the constraints at a high level and not fiddle with inner works ( e.g. provide a goal number, min and max distances for distance between position at declaration and declared goal, the evaluation order and violation action and whether the select declaration can have violations) 

## New Design approach
Create a layered system
1. Interface

Describes the common api required to trigger evaluation
1. Highlevel constraints

Defines all the inputs required for a specific constraint (e.g. goal number, min and max distances).
Creates the context layer
1. Context layer

Holds the data needed to perform a lower level constraint context layers are meant to be the glue be low and high level constraints
A particular context can be by multiple high and low level constraints
It must include the mapping between the constraint data and the high level constraint target (e.g. coordinate as data, declaration as target)
1. Low level constraints

Performs the low level check such as comparing distances against limits, calculating infringements.

### Usage in task
It might be required to group constraints for a target (e.g. all declarations constraints, all marker declaration etc) in order for the task to select the valid declaration
