apt-get update
apt-get install nano
apt-get install htop
apt-get install wget
wget https://packages.microsoft.com/config/debian/10/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
dpkg -i packages-microsoft-prod.deb
apt-get install -y apt-transport-https
apt-get install -y dotnet-sdk-5.0