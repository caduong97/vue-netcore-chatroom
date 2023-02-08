const fs = require('fs')

module.exports = {
  lintOnSave: false,

  devServer: {
    https: process.env.NODE_ENV === "production" 
    ? true
    : {
      key: fs.readFileSync('./certs/localhost.key'),
      cert: fs.readFileSync('./certs/localhost.pem')
    },
    host: 'localhost',
    port: 8080
  },

  transpileDependencies: [
    'vuetify'
  ]
}
