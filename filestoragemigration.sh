apt-get update
apt-get install git

git clone https://github.com/JingoC/FileStorageMigration

chmod -R 777 FileStorageMigration 
./FileStorageMigration/FileStorageMigration/install_runtime.sh

dotnet build FileStorageMigration/FileStorageMigration
cp FileStorageMigration/FileStorageMigration/appsettings_linux.json FileStorageMigration/FileStorageMigration/bin/Debug/netcoreapp3.1/appsettings.json

./FileStorageMigration/FileStorageMigration/bin/Debug/netcoreapp3.1/FileStorageMigration