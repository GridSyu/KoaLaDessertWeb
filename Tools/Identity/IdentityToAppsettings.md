
*** 以下是在使用 Identity 時，需要再appsettings內設定的格式： ***
------
"IdentitySettings": {
    "Roles": [
      "SuperAdmin",
      "Admin",
      "User"
    ],
    "AdminUser": {
      "Email": "Email String",
      "Password": "string",
      "Role": "SuperAdmin"
    }
  },
------