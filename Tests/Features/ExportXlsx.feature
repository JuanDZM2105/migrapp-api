Feature: Exportar reportes en formato XLSX

  Como administrador,
  Quiero exportar y descargar reportes del sistema en formato XLSX
  Para analizar la información fuera de la plataforma

  Scenario: Descarga de archivo con formato correcto
    Given que el archivo XLSX fue generado exitosamente
    Then debe contener encabezados claros y los datos organizados por columnas
    Then debe poder abrirse correctamente en Excel u otro lector de hojas de calculo
