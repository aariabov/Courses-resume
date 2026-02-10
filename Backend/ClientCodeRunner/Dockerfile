# Многоступенчатая сборка (в итоге остается только рантайм, образ весит 235Мб, против 1Гб, если использовать SDK)
# 1. Этап сборки 

# базовый образ
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# рабочая директориюя внутри контейнера — /app. Все последующие команды (например, COPY, RUN) будут выполняться в этом каталоге.
WORKDIR /src

# Копирует все файлы и папки из текущей директории (где лежит Dockerfile на хосте) в контейнер в /app. Это включает .csproj, исходники, конфиги и т. д.
COPY . . 

RUN dotnet restore

# артефакты для запуска будут в /app/out
RUN dotnet publish -c Release -o /app/out -p:PublishReadyToRun=true -p:PublishReadyToRunComposite=true


# 2. Этап рантайма
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Копирует собранное приложение из предыдущего этапа (build) в текущий образ. Это ключевой момент многоступенчатой сборки: SDK не попадает в финальный образ — только артефакты.
COPY --from=build /app/out ./

# при запуске контейнера стартуем приложение
ENTRYPOINT ["dotnet", "ClientCodeRunner.dll"]


# работает, пробую улучшить
#FROM mcr.microsoft.com/dotnet/sdk:8.0
#
#WORKDIR /app
#
#COPY . .
#
#RUN dotnet restore
#RUN dotnet publish -c Release -o out
#
#ENTRYPOINT ["dotnet", "out/ClientCodeRunner.dll"]


# долго работает
#FROM mcr.microsoft.com/dotnet/sdk:8.0
#
## Устанавливаем dotnet-script
#RUN dotnet tool install -g dotnet-script
#ENV PATH="${PATH}:/root/.dotnet/tools"
#
## Указываем точку входа
#ENTRYPOINT ["dotnet-script"]