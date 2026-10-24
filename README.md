# Tareas NoSQL - Actividad Grupal 01-U2: NoSQL App development

Integrantes:

- Yerry Maycol Tarqui Collatupa
- Milton H Flores Chino

Curso: Base de Datos II - Escuela Profesional de Ingenieria de Sistemas - Universidad Privada de Tacna

## Descripcion

Aplicacion de tareas (crear, listar, completar y eliminar) construida con ASP.NET Core (.NET 10) y **Azure Cosmos DB**, una base de datos NoSQL de documentos. La infraestructura se crea con Terraform y todo el despliegue se automatiza con GitHub Actions en Azure.

La aplicacion no usa Redis, MongoDB, Neo4j ni Prometheus, como indica el enunciado.

- Aplicacion publicada: https://app-tg1-nosql-yerrytc.azurewebsites.net
- Repositorio: https://github.com/Yerrytc/tg1-nosql-tareas-yerrytc

## Cumplimiento del enunciado

| Requisito | Como se cumple |
|---|---|
| Repositorio publico de la aplicacion | Repositorio de GitHub con el codigo, la infraestructura y los flujos |
| Despliegue mediante automatizacion | `infra.yml` (Terraform) y `deploy.yml` (publicacion de la aplicacion) en GitHub Actions |
| Publicacion en una nube publica | Azure App Service (plan gratuito F1) y Azure Cosmos DB (nivel gratuito) |
| Video de la construccion (maximo 5 minutos) | Video del proceso de construccion y funcionamiento |

## Arquitectura

```
Navegador -> Azure App Service (ASP.NET Core) -> Azure Cosmos DB (API NoSQL)
                  ^
GitHub Actions (infra.yml con Terraform, deploy.yml con dotnet publish)
```

## Estructura del proyecto

```
src/TareasApi        API y pagina web
  Models/Tarea.cs    modelo del documento
  Services/TareasRepo.cs   acceso a Cosmos DB
  Program.cs         endpoints
  wwwroot/index.html pagina web
infra/main.tf        Cosmos DB, plan de App Service y aplicacion web
.github/workflows    infra.yml y deploy.yml
evidencias           capturas de pantalla del proceso
```

## Endpoints

| Metodo | Ruta | Descripcion |
|---|---|---|
| GET | /health | estado de la aplicacion y cantidad de tareas |
| GET | /api/tareas | lista las tareas |
| GET | /api/tareas/{id} | obtiene una tarea |
| POST | /api/tareas | crea una tarea `{ "titulo": "..." }` |
| PUT | /api/tareas/{id} | actualiza `{ "titulo": "...", "hecha": true }` |
| DELETE | /api/tareas/{id} | elimina una tarea |

## Ejecucion local

Se necesita el SDK de .NET 10 y una cuenta de Azure Cosmos DB. La conexion se entrega por variables de entorno:

```bash
export Cosmos__Endpoint="https://<cuenta>.documents.azure.com:443/"
export Cosmos__Key="<llave>"
dotnet run --project src/TareasApi --urls http://localhost:5100
```

## Despliegue automatizado

1. En GitHub se guardan los secretos `ARM_CLIENT_ID`, `ARM_CLIENT_SECRET`, `ARM_SUBSCRIPTION_ID`, `ARM_TENANT_ID` y `AZURE_CREDENTIALS` de un service principal con permisos sobre el grupo de recursos.
2. `infra.yml` ejecuta Terraform (estado remoto en una cuenta de almacenamiento de Azure) y crea la cuenta de Cosmos DB, la base `tareasdb`, el contenedor `tareas`, el plan de App Service y la aplicacion web con la conexion a Cosmos DB como configuracion.
3. `deploy.yml` publica la aplicacion con `dotnet publish`, la despliega en App Service y comprueba el endpoint `/health`.

## Evidencias del proceso

### Creacion del proyecto

![Powershell en la carpeta del proyecto](evidencias/paso01_powershell_proyecto.png)

![Creacion del proyecto con dotnet new web](evidencias/paso02_dotnet_new_web.png)

![Instalacion del SDK de Cosmos DB](evidencias/paso03_paquete_cosmos.png)

### Codigo de la aplicacion

![Program.cs](evidencias/paso04_program_cs.png)

![TareasRepo.cs](evidencias/paso05_tareasrepo_cs.png)

![Tarea.cs](evidencias/paso06_tarea_cs.png)

![index.html](evidencias/paso07_index_html.png)

### Ejecucion local

![dotnet run](evidencias/paso08_dotnet_run_local.png)

![Aplicacion local sin tareas](evidencias/paso09_app_local_vacia.png)

![Aplicacion local con tareas](evidencias/paso10_app_local_tareas.png)

### Infraestructura como codigo

![main.tf](evidencias/paso11_main_tf_1.png)

![main.tf (salidas)](evidencias/paso12_main_tf_2.png)

### Automatizacion con GitHub Actions

![infra.yml](evidencias/paso13_infra_yml.png)

![deploy.yml](evidencias/paso14_deploy_yml.png)

![Repositorio](evidencias/paso21_repositorio.png)

![Comandos de git y GitHub](evidencias/paso30_git_log.png)

![Repositorio desde la consola](evidencias/paso31_gh_repo_view.png)

![Secretos configurados](evidencias/paso32_gh_secret_list.png)

![Grupo de recursos desde la consola](evidencias/paso33_az_grupo.png)

![Infraestructura en curso](evidencias/paso34_actions_infra_en_curso.png)

![Infraestructura creada](evidencias/paso18_actions_infra_exitoso.png)

![Pasos de infra](evidencias/paso19_actions_infra_pasos.png)

![Despliegue en curso](evidencias/paso15_actions_deploy_en_curso.png)

![Despliegue exitoso](evidencias/paso16_actions_deploy_exitoso.png)

![Pasos del despliegue](evidencias/paso17_actions_deploy_pasos.png)

![Lista de ejecuciones](evidencias/paso20_actions_lista.png)

### Recursos en Azure

![Grupo de recursos](evidencias/paso22_azure_grupo_recursos.png)

![Documentos en Cosmos DB](evidencias/paso23_cosmos_data_explorer.png)

### Aplicacion publicada

![Aplicacion publicada](evidencias/paso24_app_publica.png)

![Tareas agregadas](evidencias/paso25_app_publica_tareas.png)

![Tarea completada](evidencias/paso26_app_publica_completada.png)

![Tarea eliminada](evidencias/paso27_app_publica_eliminada.png)

![Endpoint /health](evidencias/paso28_health_publico.png)

![Endpoint /api/tareas](evidencias/paso29_api_tareas_publico.png)
