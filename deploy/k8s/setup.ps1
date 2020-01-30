cd C:\lhi\nsb\poc\deploy\k8s

kubectl create namespace nsb-demo
kubectl create -f prometheus-configmap.yaml -n nsb-demo
kubectl create -f nsb-demo-infra.yaml -n nsb-demo
kubectl create -f nsb-demo-apps.yaml -n nsb-demo

<#
kubectl exec prometheus-696bd7dc8c-qg77t -n nsb-demo -it ls /etc/prometheus
kubectl exec prometheus-696bd7dc8c-qg77t -n nsb-demo -it cat /etc/prometheus/prometheus.yml
#>
