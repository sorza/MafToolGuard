# MafToolGuard

Agent construído com **Microsoft Agent Framework (MAF)** utilizando *middlewares* para controlar permissões de execução de *tools*.

## Visão Geral

O **MafToolGuard** é um agente que implementa mecanismos de segurança e governança sobre ferramentas utilizadas em pipelines de agentes.  
Ele permite que cada chamada de *tool* seja interceptada e validada antes da execução, garantindo maior controle sobre permissões e prevenindo usos indevidos.

## Funcionalidades

-  **Controle de permissões**: valida se uma *tool* pode ser executada pelo agente.
-  **Integração com MAF**: utiliza o middleware `RunAsync` para interceptar chamadas.
-  **Logs estruturados**: registra tentativas de execução e bloqueios.
-  **Proteção contra uso indevido**: impede que agentes executem ações não autorizadas.
