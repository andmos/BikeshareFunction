BikeshareFunction
===

[OpenFaaS](https://www.openfaas.com/) function for [BikeshareClient](https://github.com/andmos/BikeshareClient) access.
Provides a simple serverless [GBFS](https://github.com/NABSA/gbfs) status system.

`gbfs-sysmtems-function` parses the [GBFS Systems CSV](https://raw.githubusercontent.com/NABSA/gbfs/master/systems.csv) to JSON, providing API for systems overview.

```bash
faas-cli template pull https://github.com/burtonr/csharp-kestrel-template
faas-cli build -f bikeshare-function.yml

faas-cli deploy -f bikeshare-function.yml

echo "johanneskirken" |faas-cli invoke bikeshare-function
# Bikes available: 1, Locks available: 23

echo |faas-cli invoke gbfs-systems-function |jq
#{
#    "CountryCode": "US",
#    "Name": "Sobi Long Beach",
#    "Location": "Long Beach, NY",
#    "Id": "sobi_long_beach",
#    "Url": "http://sobilongbeach.com/",
#    "GBFSFileUrl": "http://sobilongbeach.com/opendata/gbfs.json"
#  }
```

Change GBFS system by updating the `GBFSAddress` variable:

```bash
faas-cli deploy -f bikeshare-function.yml --env=GBFSAddress=https://gbfs.urbansharing.com/oslobysykkel.no/gbfs.json update=true
echo "Diakonhjemmet" |faas-cli invoke bikeshare-function
# Bikes available: 5, Locks available: 9
```