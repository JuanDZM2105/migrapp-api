Feature: Subir documento de usuario

  Como administrador
  Quiero subir un documento PNG para un usuario existente
  Para validar que la subida funciona correctamente

  Scenario: Subir un archivo PNG válido para usuario con Id 1
    Given que tengo el archivo PNG "C:\Users\Admin\Pictures\Screenshots\database.png"
    When subo el documento con Name "database.png", Type "image/png" y UserId 1
    Then la respuesta debe indicar que el documento fue subido con exito

    
  Scenario: No se permite subir archivo con formato no válido
    Given que tengo un archivo con extension ".exe" en la ruta "C:\Users\Admin\Documents\malware.exe"
    When intento subir el documento con Name "malware.exe", Type "application/x-msdownload" y UserId 1
    Then la respuesta debe indicar que la extension del archivo no esta permitida

