#!/bin/sh
set -e
rm -f linux/dcm2csv linux/dcm2csv.xz
dotnet publish dcm2csv/dcm2csv.csproj -c Release -o linux -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:DebugType=embedded -p:PublishTrimmed=false --nologo
xz -9e linux/dcm2csv
scp linux/dcm2csv.xz us:/srv/us.deadnode.org/htdocs/
