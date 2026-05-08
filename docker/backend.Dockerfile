FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/backend/DoingTasks.Api/DoingTasks.Api.csproj", "DoingTasks.Api/"]
COPY ["src/backend/DoingTasks.Application/DoingTasks.Application.csproj", "DoingTasks.Application/"]
COPY ["src/backend/DoingTasks.Domain/DoingTasks.Domain.csproj", "DoingTasks.Domain/"]
COPY ["src/backend/DoingTasks.Infrastructure/DoingTasks.Infrastructure.csproj", "DoingTasks.Infrastructure/"]
COPY ["src/backend/DoingTasks.SharedKernel/DoingTasks.SharedKernel.csproj", "DoingTasks.SharedKernel/"]
RUN dotnet restore "DoingTasks.Api/DoingTasks.Api.csproj"
COPY src/backend/ .
RUN dotnet build "DoingTasks.Api/DoingTasks.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DoingTasks.Api/DoingTasks.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DoingTasks.Api.dll"]