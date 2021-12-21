# Introdução ao Linux
## Principais diretórios
**'/' (Raíz)** Tipo c:

**/home** Diretório de trabalho dos usuários

**/root** Diretório de trabalho do super usuário

**/bin** Onde ficam os principais comandos do linux (cat, su, rm, pwd etc)

**/lib** Bibliotecas essenciais compartilhadas pelos programas e módulos do kernel

**/usr** É onde a maioria dos programas são instalados, normalmente usado com acesso apenas de leitura

**/boot** Arquivos estáticos de gerenciamento de inicialização do SO

**/etc** Arquivos de configuração e scripts de inicialização

**/sbin** Diretório de programas utilizados pelo usuário root

**/tmp** Arquivos temporários

**/var** Dados variáveis como log, dados de administração do sistema, login e arquivos transitórios

**/opt** Aplicativos adicionais e pacotes de software

**/dev** Referência dos dispositivos (hardware)

**/mnt** Pont de montagem para montar um sistema de arquivos temporariamente

## Comandos Básicos
**uname** Retorna informações sobre o kernel do SO 
**free** Dados de consumo de memória
**shudown** Reinicia e desilga a máquina
**man** Exibe o manual do comando, por exemplo ```man sudo```

## Trabalhar com arquivos e diretórios
**mkdir -p** Criar toda a árvore desejada 'pasta1/pasta2/pastafinal'
**head <arquivo>** Mostra o início do arquivo
**head <arquivo> -n 2** Limita mais ainda o tamanho
**tail <arquivo>** Mostra o final do arquivo
**tail <arquivo> -n 2** Limita mais ainda o resultado
**more <arquivo>** Mostra parte do arquivo e pode mover com a seta para ir até o final do arquivo
**less <arquivo>** Mesma coisa, mas movimenta com page up e page down
**grep <texto da busca> <arquivo>** Traz os trechos onde encontrou a ocorrência
**grep -n <texto da busca> <arquivo>** Traz o número da lina também
**grep -n -i <texto da busca> <arquivo>** Faz a busca sem diferenciar maiúsculas e minúsculas

## Execução de comandos, Redirecionamentos e Pipes
**'|'** Faz a junção de arquivos, por exemplo:

```ls -l | grep <texto>``` Filtra o resultado e e traz apenas arquivo com o termo buscado no nome

```ls -l > saida.txt``` Gera o resultado do comando em um arquivo (não traz nada no console)

```ls -a >> saida.txt``` Concatena com o resultado anterior no mesmo arquivo

### Redirecionamento
```mkdir pasta1/pasta2 2> erro.txt``` Gera a saída do erro para um arquivo.txt

## Gerenciamento de Pacotes
**DPKG** para distribuições baseadas em Debian e (arquivos .deb) **RPM** para distribuições baseadas no RedHat (arquivos .rpm)

### Instalação com download de arquivos
Instalar um aplicativo: ```sudo dpkg -i arquivo.deb```

Desinstalar um aplicativo: ```sudo dpkg -r <aplicativo>```

### Instalação sem download de arquivos
Atualizar a lista de repositórios ```sudo apt update```

Instalar um aplicativo ```sudo apt install <aplicativo>```

## Gerenciamento de Processos
Listar processos em execução: 
```ps -a``` Processos criados por todos oso usuário

Listar processos em execução com o PID: 
```ps -aux``` Processos criados por todos oso usuário

## SSH
Acesso com par de chaves, criar chave pública (atua como cadeado e vai para o servidor) e uma privada (fica na máquina).

```ssh-keygen -t rsa -b 2048```

 Na pasta  ~/.ssh/, pegar o conteúdo do aquivo .pub e registrar nas configurações de segurança no painel da aws, azure etc. Com isso vai utilizar a chave privada para validar com a chave pública

 ```ssh -i <path e arquivo da chave privada> user@IP```

# Kubernetes

## Pod
É o menor objeto do cluster kubernetes. É no pod que executamos os containers. Podemos ter mais de um container dentro de um pod.
Mas não é o correto, porque quando escalar um container vai estar escalando todo o pod.

Todos os containers compartilham a mesma rede do pod.

Pod não tem resiliência, ou seja, se cair não cria outra instancia do pod.

```yaml
apiVersion: v1
kind: Pod
metadata: 
  name: meupod
  labels:
    app: nginx
    versao: "1"
spec:
  containers:
    - name: meucontainer
      image: nginx      
```

### Comandos
Criar o pod: ```kubectl apply -f pod.yaml```

Descobrir sobre o pod: ```kubectl describe pod``` (para listar todos os pods do namespace)

Descobrir sobre o pod **(específico)**: ``` kubectl describe pod <nome do pod> ```

Deletar um pod: ```kubectl delete pod <nome do pod>```

### Bind de porta Host para Pod
``` kubectl port-forward pod/<nome do pod> 8080:80``` 

## Labels e Selectors
**Label:** Elementos chave/valor junto com metadata do objeto (versão, autor etc)

**Selectors:** Selecionar objetos baseados nos labels definidos

### Comandos
**Selecionar um pod:** ```kubectl get pods -l versao=1```

**Deletar um pod com seletor:** ```kubectl delete pod -l versao=1```

## Replicaset
Garante a quantidade de réplicas que deseja (escalabilidade e resiliência), que garanta o estado da aplicação com as réplicas necessárias.

[Documentação](https://kubernetes.io/docs/concepts/workloads/controllers/replicaset/)

### Comandos
**Criar o replicaset:** ```kubectl apply -f replicaset.yaml```

**Saber o estado do replicaset:** ```kubectl get replicaset```

**Descobrir sobre o replicaset:** ```kubectl describe replicaset``` (para listar todos os pods do namespace)


**Resiliência:** Deletando o pod criado com replicaset, automaticamente será criado outro.

Comando: ```kubectl delete pod <nome do pod>```

**Escalabilidade:** ```kubectl scale replicaset <nome do pod> --replicas=2```
Pode especificar no arquivo do manifesto a quantidade de réplicas dentro da seçao "spec" do ReplicaSet

**IMPORTANTE:** O Replicaset não gerencia a troca de versões dos containers. Para que ele consiga fazer a atualização para a nova versão, precisa deletar o(s) pod(s).

## Deployment
Gerencia os replicasets. Os deployments guarda o histórico dos replicasets, com isso pode-se fazer rollback

### Comandos
Criar o deployment: ```kubectl apply -f arquivo.yaml```

Saber o estado do deployment: ```kubectl get deployment```

Descobrir sobre o deployment: ```kubectl describe deployment``` (para listar todos os pods do namespace)

**Para ver as versões:** ```kubectl rollout history deployment <nome do deployment>```

**Para fazer rollback para a versão anterior:** ```kubectl rollout undo reployment <nome do deployment>```


No rollback o deployment aproveita o replicaset anterior, com isso não faz novo deploy.

**Para voltar para a versão atual novamente:** ```kubectl set image deployment <nome do deployment> <nome do container>=<imagem>```

### R E S U M Ã O (Pod, ReplicaSet e Deployment)
O **Pod** é o menor elemento do cluster. Nele podeos ter um ou mais containers. Rodar mais de um container é para o caso de o container principal precisar de aplicações de apoio (por exemplo, coleta de log em arquivos gravado pelo container principal do pod), mas geralmente é utilizado um container por pod.

O **Pod** por si só não tem resiliência, ao excluir um pod ele não será recriado automaticamente, para isso usamos o **ReplicaSet**. É ele que gerencia os pods e permite criar replicas dos pods. O Replicaset também dá resiliência, pois ao deletar um pod ele sobe outro automaticamente, mantendo sempre as replicas solicitadas na sua criação.

O **RepplicaSet** não gerencia atualizações de verssão dos pods de forma automática, para isso utilizamos o **Deployment**. Com o deployment, a cada nova versão ele faz a troca automática da versão antiga pela nova, mantendo a quantidade de réplicas solicitadas na sua criação.

O **Deployment** persiste as versões já atualizadas permitindo assim fazer um rollback para versões anteriores, sem fazer um novo deployment.



## Objetos Services
[Introdução](https://kubernetes.io/pt-br/docs/tutorials/kubernetes-basics/expose/expose-intro/) | [Documentção](https://kubernetes.io/docs/concepts/services-networking/service/)

### ClusterIP
Serve para gerar conexão entre os pods **dentro** do cluster (nada externo).

[Documentação](https://kubernetes.io/docs/concepts/services-networking/service/#publishing-services-service-types)


### NodePort
Gera comunicação com um porta externa de 30000 a 32767.

[Documentação](https://kubernetes.io/docs/concepts/services-networking/service/#type-nodeport)

### LoadBalancer
Utiliza o provedor para obter um IP (só funciona em ambiente cloud). Utilizado em serviços de cloud.

[Documentação](https://kubernetes.io/docs/concepts/services-networking/service/#loadbalancer)

### ExternalName
Para gerar um padronização com o meio externo. Tipo DNS.

[Documentação](https://kubernetes.io/docs/concepts/services-networking/service/#externalname)


## End Points
Entre o service e o pod, quando cria um service e usa os selectors para vincular ao pod, outro objeto também é
criado por debaixo dos panos, esse objeto é o Endpoint. Nada mais é do que uma cloeção com todos os pods que são vinculados a esse service.

Esses são criados de forma automática, mas tem como criar manualmente também.

```kubectl get endpoints```

## Namespaces
Cria uma separação lógica dentro do cluster. Serve por exemplo para separar ambientes (DEV, QA etc)

[Documentação](https://kubernetes.io/docs/concepts/overview/working-with-objects/namespaces/)

### Comandos
**Listar namespaces:** ```kubectl get namespaces```

**Listar deployments em um namespace:** ```kubectl get deployments -n <namespace>```

**Listar todos os deployments:** ```kubectl get deployments --all-namespaces```

**Fazendo o deploy especificando o namespace:** ```kubectl apply -f <arquivo>.yaml -n <namespace>```

**Criar namespace após o deploy**: ```kubectl create namespace <namespace>```

**Criar namespace no yaml:** definir o namespace no metadata

### Comunicação entre namespaces (Externamente)
Em um arquivo de service.yaml e aplicar o manifesto nos namespaces.

```kubectl apply -f service.yaml -n <namespace>```

Fazer isso para todos os namespaces desejados.

### Comunicação entre namespaces (entre pods)
#### Abordagem por IP 
Listar os pods para pegar o IP
```kubectl get pods --all-namespaces -o wide``` e acessar com o IP de dentro de um pod através do IP listado

#### Abordagem por nome de service
```http://<nome do service>.<namespace>.svc.cluster.local```

#### Simplicando a comunicação
Criar um service do tipo 'ExternalName' e colocar da propriedade externalName do 'spec' o nome utilizado acima, mas sem http. Repetir isso para cada namespace.

#### O que é separado por namespaces e o que não é ?
```kubectl api-resources --namespaced=true``` vai listar todos os tipos que podem ser separados por namespace

## Subindo uma aplicação
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



