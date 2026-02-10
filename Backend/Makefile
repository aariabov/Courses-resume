# Инструкция по dotnet
# https://learn.microsoft.com/en-us/ef/core/cli/dotnet

.PHONY: scaffold
scaffold:
	dotnet ef dbcontext scaffold "Name=ConnectionStrings:Devpull" Npgsql.EntityFrameworkCore.PostgreSQL --context AppDbContext --output-dir DbModels --force --project Devpull

.PHONY: db_new
db_new:
	dotnet ef migrations add $(name) --project Devpull
	
.PHONY: db_up
db_up:
	dotnet ef database update --project Devpull
	
.PHONY: db_status
db_status:
	dotnet ef migrations list --project Devpull

# посмотреть SQL, https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli
# чтобы посмотреть SQL, начиная с определенной миграции, надо задать ее название, например dotnet ef migrations script --project Devpull CreatePaymentTable
.PHONY: db_script
db_script:
	dotnet ef migrations script --project Devpull

# удалить последнюю миграцию
.PHONY: db_remove
db_remove:
	dotnet ef migrations remove --project Devpull

# запустить для e2e тестов
.PHONY: e2e
e2e:
	dotnet run environment=Testing --project Devpull

# запустить бек для разработки
.PHONY: run_back
run_back:
	dotnet run environment=Development --project Devpull


# build client code Docker image
.PHONY: docker_build
docker_build:
	docker build -t csharp-runner:latest ./clientcoderunner

# build client code Docker image
.PHONY: docker_tester_build
docker_tester_build:
	docker build -t csharp-tester:latest -f tester.Dockerfile .
