# DoingTasks
 Application to task management in general.


## Commands to create the initial structure
 - dotnet new webapi -n DoingTasks.Api -f net10.0
 - dotnet new classlib  -n DoingTasks.Application -f net10.0
 - dotnet new classlib  -n DoingTasks.Domain -f net10.0
 - dotnet new classlib  -n DoingTasks.Infrastructure -f net10.0
 - dotnet new classlib  -n DoingTasks.SharedKernel -f net10.0

 - dotnet new xunit -n DoingTasks.Application.UnitTests -f net10.0
 - dotnet new xunit -n DoingTasks.Domain.UnitTests -f net10.0
 - dotnet new xunit -n DoingTasks.Infrastructure.UnitTests -f net10.0
 - dotnet new xunit -n DoingTasks.IntegrationTests -f net10.0

 - ng new doing-tasks --standalone --routing --style=scss --skip-git
 
 - touch docker/backend.Dockerfile
 - touch docker/frontend.Dockerfile
 - touch docker-compose.yml
 - touch docker-compose.override.yml
