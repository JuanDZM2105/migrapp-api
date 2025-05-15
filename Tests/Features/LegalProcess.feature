Feature: Consulta de procesos legales de un usuario

  Como administrador
  Quiero consultar la lista de procesos legales asociados a un usuario
  Para validar que el endpoint funciona correctamente y devuelve la información esperada

  Scenario: Obtener procesos legales para usuario con Id 1
    When consulto los procesos legales del usuario con Id 1
    Then la respuesta debe ser exitosa
    And la respuesta debe contener una lista con objetos de proceso legal validos
