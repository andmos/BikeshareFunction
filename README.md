BikeshareFunction
===

[OpenFaaS](https://www.openfaas.com/) function for [BikeshareClient](https://github.com/andmos/BikeshareClient) access.
Provides a simple serverless [GBFS](https://github.com/NABSA/gbfs) status system.

```bash
faas-cli template pull https://github.com/burtonr/csharp-kestrel-template
faas-cli build -f bikeshare-function.yml

faas-cli deploy -f bikeshare-function.yml

echo "johanneskirken" |faas-cli invoke bikeshare-function
# Bikes available: 1, Locks available: 23
```

Change GBFS system by updating the `GBFSAddress` variable:

```bash
faas-cli deploy -f bikeshare-function.yml --env=GBFSAddress=https://gbfs.urbansharing.com/oslobysykkel.no/gbfs.json update=true
echo "Diakonhjemmet" |faas-cli invoke bikeshare-function
# Bikes available: 5, Locks available: 9
```