Установка и запуск приложения

Зайти в приложение filestorage в kubernetes

1. Установить git

apt-get update
apt-get install git

2. Клонировать утилиту FileStorageMigration

git clone https://github.com/JingoC/FileStorageMigration

3. Дать права клонированной директории и запустить инсталлятор

chmod -R 777 FileStorageMigration 
./FileStorageMigration/FileStorageMigration/install_runtime.sh

4. Собрать приложение

dotnet build FileStorageMigration/FileStorageMigration

5. Копировать конфигурационный файл в директорию запуска

cp FileStorageMigration/FileStorageMigration/appsettings_linux.json FileStorageMigration/FileStorageMigration/bin/Debug/netcoreapp3.1/appsettings.json

6. Запуск утилиты

./FileStorageMigration/FileStorageMigration/bin/Debug/netcoreapp3.1/FileStorageMigration