# Pod
É o menor objeto do cluster kubernetes. É no pod que executamos os containers. Podemos ter mais de um container dentro de um pod.
Mas não é o correto, porque quando escalar um container vai estar escalando todo o pod.

Todos os containers compartilham a mesma rede do pod.

Pod não tem resiliência, ou seja, se cair não cria outra instancia do pod

## Comandos
Criar o pod: ```kubectl apply -f pod.yaml```

Descobrir sobre o pod: ```kubectl describe pod``` (para listar todos os pods do namespace)

Descobrir sobre o pod **(específico)**: ``` kubectl describe pod <nome do pod> ```

## Bind de porta Host para Pod
``` kubectl port-forward pod/<nome do pod> 8080:80``` 

# Labels e Selectors
**Label:** Elementos chave/valor junto com metadata do objeto (versão, autor etc)

**Selectors:** Selecionar objetos baseados nos labels definidos

## Comandos
**Selecionar um pod:** ```kubectl get pods -l versao=1```

**Deletar um pod com seletor:** ```kubectl delete pod -l versao=1```

# Replicaset
Garante a quantidade de réplicas que deseja (escalabilidade e resiliência), que garanta o estado da aplicação com as réplicas necessárias.

[Documentação](https://kubernetes.io/docs/concepts/workloads/controllers/replicaset/)

## Comandos
**Criar o replicaset:** ```kubectl apply -f replicaset.yaml```

**Saber o estado do replicaset:** ```kubectl get replicaset```

**Descobrir sobre o replicaset:** ```kubectl describe replicaset``` (para listar todos os pods do namespace)


**Resiliência:** Deletando o pod criado com replicaset, automaticamente será criado outro.

Comando: ```kubectl delete pod <nome do pod>```

**Escalabilidade:** ```kubectl scale replicaset <nome do pod> --replicas=2```
Pode especificar no arquivo do manifesto a quantidade de réplicas dentro da seçao "spec" do ReplicaSet

**IMPORTANTE:** O Replicaset não gerencia a troca de versões dos containers. Para que ele consiga fazer a atualização para a nova versão, precisa deletar o(s) pod(s).

# Deployment
Gerencia os replicasets. Os deployments guarda o histórico dos replicasets, com isso pode-se fazer rollback

## Comandos
Criar o deployment: ```kubectl apply -f arquivo.yaml```

Saber o estado do deployment: ```kubectl get deployment```

Descobrir sobre o deployment: ```kubectl describe deployment``` (para listar todos os pods do namespace)

**Para ver as versões:** ```kubectl rollout history deployment <nome do deployment>```

**Para fazer rollback para a versão anterior:** ```kubectl rollout undo reployment <nome do deployment>```


No rollback o deployment aproveita o replicaset anterior, com isso não faz novo deploy.

**Para voltar para a versão atual novamente:** ```kubectl image deployment <nome do deployment> <nome do container>=<imagem>```

# Objetos Services
[Introdução](https://kubernetes.io/pt-br/docs/tutorials/kubernetes-basics/expose/expose-intro/)

[Documentção](https://kubernetes.io/docs/concepts/services-networking/service/)

## ClusterIP
Serve para gerar conexão entre os pods **dentro** do cluster (nada externo).
[Documentação](https://kubernetes.io/docs/concepts/services-networking/service/#publishing-services-service-types)


## NodePort
Gera comunicação com um porta externa de 30000 a 32767.
[Documentação](https://kubernetes.io/docs/concepts/services-networking/service/#type-nodeport)

## LoadBalancer
Utiliza o provedor para obter um IP (só funciona em ambiente cloud). Utilizado em serviços de cloud.
[Documentação](https://kubernetes.io/docs/concepts/services-networking/service/#loadbalancer)

## ExternalName
Para gerar um padronização com o meio externo. Tipo DNS.
[Documentação](https://kubernetes.io/docs/concepts/services-networking/service/#externalname)


# End Points
Entre o service e o pod, quando cria um service e usa os selectors para vincular ao pod, outro objeto também é
criado por debaixo dos panos, esse objeto é o Endpoint. Nada mais é do que uma cloeção com todos os pods que são vinculados a esse service.

Esses são criados de forma automática, mas tem como criar manualmente também.

```kubectl get endpoints```

# Namespaces
Cria uma separação lógica dentro do cluster. Serve por exemplo para separar ambientes (DEV, QA etc)
[Documentação](https://kubernetes.io/docs/concepts/overview/working-with-objects/namespaces/)

## Comandos
**Listar namespaces:** ```kubectl get namespaces```

**Listar deployments em um namespace:** ```kubectl get deployments -n <namespace>```

**Listar todos os deployments:** ```kubectl get deployments --all-namespaces```

**Fazendo o deploy especificando o namespace:** ```kubectl apply -f <arquivo>.yaml -n <namespace>```

**Criar namespace após o deploy**: ```kubectl create namespace <namespace>```

**Criar namespace no yaml:** definir o namespace no metadata

## Comunicação entre namespaces (Externamente)
Em um arquivo de service.yaml e aplicar o manifesto nos namespaces.

```kubectl apply -f service.yaml -n <namespace>```

Fazer isso para todos os namespaces desejados.

## Comunicação entre namespaces (entre pods)
### Abordagem por IP 
Listar os pods para pegar o IP
```kubectl get pods --all-namespaces -o wide``` e acessar com o IP de dentro de um pod através do IP listado

### Abordagem por nome de service
```http://<nome do service>.<namespace>.svc.cluster.local```

### Simplicando a comunicação
Criar um service do tipo 'ExternalName' e colocar da propriedade externalName do 'spec' o nome utilizado acima, mas sem http. Repetir isso para cada namespace.

### O que é separado por namespaces e o que não é ?
```kubectl api-resources --namespaced=true``` vai listar todos os tipos que podem ser separados por namespace

# Subindo uma aplicação
1. git clone da aplicação
2. Ter um Dockerfile de criação da imagem
3. Criação da imagem
4. Push da imagem
5. Criar os manifestos
    5.1 Crias uma pasta k8s
    5.2 Manifesto de **Deployment** da aplicação e das dependências (mongoDb, por exemplo)
    Repetir para todas as aplicações que desejar criar
    5.3 Criar um service do tipo **ClusterIp** para permitir por exemplo a comunicação com o mongoDb através da aplicação
    Definir **imagePullPolicy** na definição do container no manifesto
6. Criar um **Service** do tipo **NodePort**
7. Aplicar os manifestos



