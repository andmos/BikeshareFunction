BikeshareFunction
===

[OpenFaaS](https://www.openfaas.com/) function for [BikeshareClient](https://github.com/andmos/BikeshareClient) access.
Provides a simple serverless [GBFS](https://github.com/NABSA/gbfs) status system.

`gbfs-systems-function` parses the [GBFS Systems CSV](https://raw.githubusercontent.com/NABSA/gbfs/master/systems.csv) to JSON, providing API for systems overview.

```bash
faas-cli template pull https://github.com/burtonr/csharp-kestrel-template
faas-cli build -f bikeshare-function.yml

faas-cli deploy -f bikeshare-function.yml

echo "johanneskirken" |faas-cli invoke bikeshare-function |jq
# {
#  "BikesAvailable": 1,
#  "LocksAvailable": 23
# }

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
# {
#  "BikesAvailable": 9,
#  "LocksAvailable": 5
# }
```

This project also contains function to post from `bikeshare-function` to Slack. Add a new bot named `bikesharebot` to a Slack workspace, and update the variable under `bikeshare-slack-function:` in `bikeshare-function.yml` with the location of the [OpenFaaS gateway](https://github.com/openfaas/workshop/blob/master/lab4.md#call-one-function-from-another):

```yaml
environment:
    gateway_hostname: http://gateway:8080/
```

Add the bot's OAUTH token to [OpenFaaS secrets](https://docs.openfaas.com/reference/secrets/):

```shell
faas-cli secret create bikeBotSlackToken --from-file=slackbot-key.txt
```

and initialize the bot by trigging the function after deploy:

```shell
echo "init" |faas-cli invoke bikeshare-slack-function
# Bot initializing
```

Now simply ask the bot for a station:

```shell
andmos [8:40 PM]
@BikeshareBot Lerkendal
BikeshareBot APP [8:40 PM]
:bike:: 17 :unlock:: 3
```

To run OpenFaaS, look at the guides for [Docker Swarm](https://docs.openfaas.com/deployment/docker-swarm/), [Kubernetes](https://docs.openfaas.com/deployment/kubernetes/) or [OpenShift](https://docs.openfaas.com/deployment/openshift/).

[![Build Status](https://travis-ci.org/andmos/BikeshareFunction.svg?branch=master)](https://travis-ci.org/andmos/BikeshareFunction)