<h2>Установка и запуск приложения</h2>

<h4>Зайти в приложение filestorage в kubernetes</h4>

Тестовый стенд: `https://kubeboard.npc.ba/#/shell/ros-patent-test/visary-filestorage-ros-patent-test-85468897f-jl88t/visary-filestorage?namespace=ros-patent-test`

<h4>1. Установить git</h4>

`apt-get update`

`apt-get install git`

<h4>2. Клонировать утилиту FileStorageMigration</h4>

`git clone https://github.com/JingoC/FileStorageMigration`

<h4>3. Дать права клонированной директории и запустить инсталлятор</h4>

`chmod -R 777 FileStorageMigration`

`./FileStorageMigration/FileStorageMigration/install_runtime.sh`

<h4>4. Собрать приложение</h4>

`dotnet build FileStorageMigration/FileStorageMigration`

<h4>5. Копировать конфигурационный файл в директорию запуска</h4>

`cp FileStorageMigration/FileStorageMigration/appsettings_linux_replace.json appsettings.json`

<h4>6. Запуск утилиты</h4>

`./FileStorageMigration/FileStorageMigration/bin/Debug/netcoreapp3.1/FileStorageMigration`
