# DoingTasks
 Application to task management in general.

 ## Commands
 ### Commands to create the initial structure
  -dotnet new webapi -n DoingTasks.Api -f net10.0
  -dotnet new classlib  -n DoingTasks.Application -f net10.0
  -dotnet new classlib  -n DoingTasks.Domain -f net10.0
  -dotnet new classlib  -n DoingTasks.Infrastructure -f net10.0
  -dotnet new classlib  -n DoingTasks.SharedKernel -f net10.0
 
  -dotnet new xunit -n DoingTasks.Application.UnitTests -f net10.0
  -dotnet new xunit -n DoingTasks.Domain.UnitTests -f net10.0
  -dotnet new xunit -n DoingTasks.Infrastructure.UnitTests -f net10.0
  -dotnet new xunit -n DoingTasks.IntegrationTests -f net10.0
 
  - ng new doing-tasks --standalone --routing --style=scss --skip-git
  
  -touch docker/backend.Dockerfile
  -touch docker/frontend.Dockerfile
  -touch docker-compose.yml
  -touch docker-compose.override.yml

 ### Commands for packages used
  -dotnet add src/backend/DoingTasks.Infrastructure package Microsoft.EntityFrameworkCore
  -dotnet add src/backend/DoingTasks.Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL
  -dotnet add src/backend/DoingTasks.Infrastructure package Microsoft.EntityFrameworkCore.Design
  -dotnet add src/backend/DoingTasks.Api package Microsoft.EntityFrameworkCore.Design

 ### Commands for dotnet and migrations
  -dotnet tool install dotnet-ef
  -dotnet ef migrations add InitialCreate --project src/backend/DoingTasks.Infrastructure --startup-project  src/backend/DoingTasks.Api
  -dotnet ef database update --project src/backend/DoingTasks.Infrastructure --startup-project src/backend/ DoingTasks.Api


 ### Docker commands used
  #### Start all services
   -docker-compose up
 
 #### Start in background mode (most common for daily use)
  -docker-compose up -d
 
 #### View logs
  -docker-compose logs -f
 
 #### Stop and remove everything
  -docker-compose down
 
 ## Business Rules

 ### User Management
 
 - **Age Requirement**: Users must be at least 18 years old at the time of account creation.
 - **Email Validation**: Each user must have a valid, non-empty email address.
 - **Full Name**: A valid, non-empty full name is required for all users.
 - **Nickname**: Each user must have a nickname (username) that follows validation constraints (required and length-limited).
 - **Profile Updates**: Users can update their full name, nickname, and birth date, subject to the same validation rules as account creation.
 
 ### Workspace Management
 
 - **Ownership**: Each workspace has a single owner who can be the creator of the workspace. Only the owner can perform administrative operations.
 - **Naming**: Workspaces must have a non-empty, valid name. Workspace names can be renamed only by the owner.
 - **Grouping**: Workspaces can optionally be assigned to a visual group (groupName) for organizational purposes in the frontend.
 - **Operational Requirement**: A workspace must have a minimum of 2 workflow states to be considered operational.
 - **Minimum States**: Every workspace must contain at least one initial state and one terminal state to function properly.
 - **Initial State**: The initial state of a workspace is always the state with the lowest order value.
 
 ### Workspace State Management
 
 - **State Ordering**: States must follow a sequential order without gaps. Each state must have a unique order number within the workspace.
 - **State Transitions**: When a state is removed, all subsequent states are automatically reordered to maintain the sequence integrity.
 - **State Naming**: Workspace states must have non-empty, valid names. Only the workspace owner can rename states.
 - **State Addition**: Only the workspace owner can add new states to the workspace.
 - **State Removal**: Only the workspace owner can remove states from the workspace.
 - **State Reordering**: Only the workspace owner can change the order of states within the workspace.
 
 ### Workspace Collaboration
 
 - **Collaborator Permissions**: Workspace owners can toggle whether collaborators are allowed to edit tasks and their contents.
 - **Member Management**: Only the workspace owner can invite users to join the workspace.
 - **Member Roles**: Workspace members are assigned specific roles (e.g., Collaborator, Viewer) that determine their permissions.
 - **Duplicate Membership**: A user cannot be invited to the same workspace multiple times; duplicate memberships are prevented.
 - **Role Changes**: Only the workspace owner can modify the role of existing workspace members.
 - **Member Removal**: Only the workspace owner can remove members from the workspace.
 
 ### Task Management
 
 - **Task Creation**: Each task must belong to a specific workspace and have a non-empty, valid title.
 - **Task State**: A task must be assigned to a workspace state. Tasks can only transition between valid states within the workspace.
 - **Task Assignment**: Tasks can be assigned to a user or remain unassigned.
 - **Task Blocking**: A task can be blocked with a mandatory justification. Tasks cannot be blocked if they are already blocked.
 - **Task Unblocking**: A task can only be unblocked if it is currently in a blocked state. Once unblocked, the block justification is removed.
 - **Task Transitions**: Blocked tasks cannot transition to a different state until they are unblocked.
 - **Task Complexity**: Task complexity is optional and must be a valid value if provided.
 - **Planned Hours**: The planned hours for a task are optional and can be updated independently.
 - **Task Comments**: Users can add, update, and remove comments on tasks to facilitate collaboration and discussion.
 
 ### Task Steps (Subtasks)
 
 - **Step Creation**: Each step must have a non-empty, valid title and must belong to a specific task and workspace state.
 - **Step Assignment**: Steps can be assigned to a user or remain unassigned.
 - **Step State Requirement**: Steps can only transition to "Doing" or "Done" states if the parent task is in the matching workspace state.
 - **Step Completion**: When a step is completed, the actual hours spent must be recorded as a non-negative value.
 - **Automatic Hour Update**: When a step is marked as complete, the parent task's actual hours are automatically updated to reflect the sum of all completed  step hours.
 - **Hours Validation**: The total actual hours of a task cannot be less than the sum of all its steps' actual hours.
 
 ### Task Comments
 
 - **Comment Creation**: Comments must have non-empty content and must be authored by a valid user.
 - **Comment Updates**: Existing comments can be updated with new content.
 - **Comment Removal**: Comments can be removed from a task.
 
 ### Auditing and Tracking
 
 - **Workspace Auditing**: All administrative actions performed on workspaces are logged for audit purposes.
 - **Audit Actions**: Supported audit actions include creation, updates, member invitations, and role changes.
 - **Event Tracking**: Domain events are raised for all significant business operations (e.g., task creation, state transitions, blocking).
 
 ### Data Integrity Invariants
 
 - **Workspace Consistency**: A workspace must maintain a consistent set of states in sequential order without gaps.
 - **Task-Step Consistency**: A task's actual hours must always be greater than or equal to the sum of its steps' actual hours.
 - **State-Task Consistency**: Tasks can only be in states that exist within their assigned workspace.
 - **Step-State Consistency**: Steps can only reference workspace states that exist within their parent task's workspace.
 - **Member Consistency**: Workspace members must be unique; no duplicate user IDs in the membership list.