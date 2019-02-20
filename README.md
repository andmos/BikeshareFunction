BikeshareFunction
===

[OpenFaaS](https://www.openfaas.com/) function for [BikeshareClient](https://github.com/andmos/BikeshareClient) access.

```bash
faas-cli template pull https://github.com/burtonr/csharp-kestrel-template
faas-cli build -f bikeshare-function.yml

faas-cli deploy -f bikeshare-function.yml

echo "johanneskirken" |faas-cli invoke bikeshare-function
# Bikes available: 1, Locks available: 23
```
