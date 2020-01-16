break > ..\..\Changeset.txt
git rev-parse HEAD >> ..\..\Changeset.txt 
echo. >> ..\..\Changeset.txt 
git rev-parse --short HEAD >> ..\..\Changeset.txt