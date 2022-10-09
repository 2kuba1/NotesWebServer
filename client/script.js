 fetch("https://localhost:7172/api/account/getString")
   .then(response => response.text())
   .then((response) => {
       console.log(response)
   })
   .catch(err => console.log(err))