# kestrelssl

You will need a valid SSL certificate signed by a public certificate authority. For obvious reasons I cannot include our own certificate with a private key in this repo.

Make sure certificate is saved in the `sslcertificate.pfx` file

### Install the tooling

- ./bootstrap

### Build and deploy service

- ./build
- ./deploy

### Check that service responds to Https requests

- start `minikube tunnel` in a separate terminal
- Fetch public ip `ip=$(kubectl get service kestrelssl -o json | jq ".status.loadBalancer.ingress[0].ip" | tr -d '"')`
- `curl https://foo.mydomain.com --resolve "foo.mydomain.com:443:$ip"`

You should get instantenious response from the server printing out full SSL certificate chain (usually containing 3 certificates)

### Block resolving certificate AIA

- Apply network policy that blocks DNS  `kubectl apply -f netpolicy.yml`
- Restart pods `kubectl rollout restart deployment kestrelssl`

### Check that service fails Https requests

- `curl https://foo.mydomain.com --resolve "foo.mydomain.com:443:$ip"`

You should see curl failing to connect to the service (you might need to wait for the service finishing initializing which is 15 sec)
```
curl: (60) SSL certificate problem: unable to get local issuer certificate
More details here: https://curl.haxx.se/docs/sslcerts.html

curl failed to verify the legitimacy of the server and therefore could not
establish a secure connection to it. To learn more about this situation and
how to fix it, please visit the web page mentioned above.
```

However it will respond if you add `--insecure`. Observe that requests become very slow. It's because they try building `X509Chain` which has 15 sec timeout. Also observe that response now contain only one (leaf) certificate in the chain.

