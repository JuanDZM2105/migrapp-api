Feature: Login con OTP

Scenario: Usuario se logea con credenciales y OTP válidos
  Given el usuario abre la página de login
  When ingresa el email "user@ejemplo.com" y la contraseña "123456"
  And se le envía un código OTP
  And el usuario ingresa el código "789456"
  Then debería ver el mensaje "Bienvenido"

Scenario: Usuario ingresa OTP inválido
  Given el usuario abre la página de login
  When ingresa el email "admin@ejemplo.com" y la contraseña "123456"
  And se le e´nvía un código OTP
  And el usuario ingresa el código "000000"
  Then debería ver el mensaje "Código OTP inválido"

Scenario: Usuario ingresa credenciales incorrectas
  Given el usuario abre la página de login
  When ingresa el email "otro@correo.com" y la contraseña "claveIncorrecta"
  Then debería ver el mensaje "Credenciales incorrectas"
