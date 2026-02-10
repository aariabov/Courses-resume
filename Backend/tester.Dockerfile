# Т.к используется Common и лежит в другой папке, docker file перенес на тот же уровень, иначе не видит Common

# Многоступенчатая сборка (в итоге остается только рантайм, образ весит 235Мб, против 1Гб, если использовать SDK)
# 1. Этап сборки 

# базовый образ
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# рабочая директориюя внутри контейнера — /app. Все последующие команды (например, COPY, RUN) будут выполняться в этом каталоге.
WORKDIR /src

# Копирует все файлы и папки из текущей директории (где лежит Dockerfile на хосте) в контейнер в /app. Это включает .csproj, исходники, конфиги и т. д.
COPY ExerciseTester ./ExerciseTester

# копируем общий проект
COPY Common ./Common

# восстанавливаем зависимости
RUN dotnet restore ExerciseTester/ExerciseTester.csproj

# артефакты для запуска будут в /app/out
RUN dotnet publish ExerciseTester/ExerciseTester.csproj -c Release -o /app/out -p:PublishReadyToRun=true -p:PublishReadyToRunComposite=true


# 2. Этап рантайма
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Копирует собранное приложение из предыдущего этапа (build) в текущий образ. Это ключевой момент многоступенчатой сборки: SDK не попадает в финальный образ — только артефакты.
COPY --from=build /app/out ./

# при запуске контейнера стартуем приложение
ENTRYPOINT ["dotnet", "ExerciseTester.dll"]
