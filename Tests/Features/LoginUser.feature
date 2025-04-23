Feature: Login con OTP

Scenario: Usuario se logea con credenciales y OTP v�lidos
  Given el usuario abre la p�gina de login
  When ingresa el email "user@ejemplo.com" y la contrase�a "123456"
  And se le env�a un c�digo OTP
  And el usuario ingresa el c�digo "789456"
  Then deber�a ver el mensaje "Bienvenido"

Scenario: Usuario ingresa OTP inv�lido
  Given el usuario abre la p�gina de login
  When ingresa el email "admin@ejemplo.com" y la contrase�a "123456"
  And se le e�nv�a un c�digo OTP
  And el usuario ingresa el c�digo "000000"
  Then deber�a ver el mensaje "C�digo OTP inv�lido"

Scenario: Usuario ingresa credenciales incorrectas
  Given el usuario abre la p�gina de login
  When ingresa el email "otro@correo.com" y la contrase�a "claveIncorrecta"
  Then deber�a ver el mensaje "Credenciales incorrectas"
