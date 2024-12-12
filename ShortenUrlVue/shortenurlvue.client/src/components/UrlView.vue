<template>
    <div class="url-view">
    <div class="container">
        <div class="form-box">
            <h2>Shorten your URL</h2>
            <form>
                <!-- <label for="name">Name:</label> -->
                <input ref="my_input" type="text" id="longname" name="longname" placeholder="Enter your long URL" required>
                
                <!-- <label for="email">Email:</label> -->
                <!-- <input type="text" id="shortname" name="shortname" placeholder="Enter your shortname (Optional)" required> -->
                
                <!-- <label for="message">Message:</label> -->
                <!-- <textarea id="message" name="message" placeholder="Enter your message" rows="4" required></textarea> -->
                
                <button form @click.prevent="getFormValues()" type="submit">Submit</button>
            </form>
        </div>
        <div class="list-box">
            <h2>List of Items</h2>
            <ul>
                <li v-for="url in urlList" ><div class="url-box">{{url.shortUrl}}</div></li>
            </ul>
        </div>
      <div>
        <button></button>
      </div>
    </div>
</div>
</template>

<script>
import { errorMessages } from "vue/compiler-sfc";

export default {
  name: 'UrlView',
  data() {
    return {
        apiUrlTest: "https://localhost:8888",
      urlpost: "",
      urlput: "",
      urldelete: "",
      urlList: [],
      code: ""

         
    }
  },
  props: {
  },
  methods: {
    async postData(urlpost) {
        try {
            const res = await fetch(this.apiUrlTest + "/UrlShorten/Shorten", {
            method: "POST",
            headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
            },
            body: JSON.stringify({ url: urlpost },),
          })

          if (!res.ok) {
                  throw new Error(`Response status: ${response.status}`);
                }
                const finalRes = await res.json();
                console.log(finalRes)

        } catch (error) {
          console.error(error.message);
        }
    },

    //Get
    async getDataTest() {
      try {
        const res = await fetch(this.apiUrlTest + "/UrlShorten");
        if (!res.ok) {
          throw new Error(`Response status: ${res.status}`);
        }
        const finalRes = await res.json();
        console.log(finalRes)
        this.urlList = finalRes;
      } catch (error) {
        console.error(error.message);
      }
    },


    //Get Qr Code
    async getQrCode() {
      try {
        const res = await fetch(this.apiUrlTest + "/UrlShorten/qrcode/" + this.code);
        if (!res.ok) {
          throw new Error(`Response status: ${res.status}`);
        }
        const finalRes = await res.json();
        console.log(finalRes)
      } catch (error) {
        console.error(error.messages)
      }
    },

    //Edit Shorten Code
    async putShortenCode() {
      try {
        const res = await fetch(this.apiUrlTest + "/UrlShorten/edit-shorten?code=" + this.code)
        if (!res.ok) {
          throw new Error(`Response status: ${res.status}`);
        }
        const finalRes = await res.json();
        console.log(finalRes)
      } catch (error) {
        console.log(error)
      }
    },


    //Edit Origin
    async editOriginCode(urlput) {
      try {
        const res = await fetch(`${this.apiUrlTest}/UrlShorten/edit-origin?code=${this.code}`, {
          method: "PUT",
          headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
          },
          body: JSON.stringify({ url: urlput })
        });
        const finalRes = await res.json();
        console.log(finalRes);
      } catch (error) {
        console.error('Error:', error.message);
      }
    },


    //Delete Shorten
    async deleteShortenCode(urldelete) {
      try {
        const res = await fetch(this.apiUrlTest + "/UrlShorten/delete-shorten", {
          method: "DELETE",
          headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
          },
          body: JSON.stringify({ url: urldelete })
        });
        const finalRes = await res.json();
        console.log(finalRes);
      } catch (error) {
        console.error('Error:', error.message);
      }
    },



      
    },

      getFormValues () {
        this.urlpost = this.$refs.my_input.value
        console.log(this.urlpost)
        this.postData(this.urlpost)
    },

    mounted() {
      this.getDataTest()
    }

 }



</script>

<style scoped>
.url-view {
    display: flex;
    justify-content: center;
    align-items: center;
    height: 100vh;
    margin: 0;
    font-family: Arial, sans-serif;
    /* background-color: #f4f4f9; */
}



.container {
    display: flex;
    width: 80%;
    max-width: 1200px;
    gap: 20px;
}
.form-box, .list-box {
    flex: 1;
    padding: 20px;
    border: 1px solid #ccc;
    border-radius: 5px;
    background-color: #fff;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    
}



.form-box form {
    display: flex;
    flex-direction: column;
    gap: 10px;
}
.form-box input, .form-box button {
    padding: 10px;
    border: 1px solid #ccc;
    border-radius: 3px;
}
.form-box button {
    background-color: #007bff;
    color: #fff;
    border: none;
    cursor: pointer;
}
.form-box button:hover {
    background-color: #0056b3;
}
.list-box ul {
    list-style: none;
    padding: 0;
    margin: 0;
}
.list-box ul li {
    padding: 8px;
    border-bottom: 1px solid #eee;
}
.list-box ul li:last-child {
    border-bottom: none;
}

ul {
    overflow:hidden;
    overflow-y:scroll;
    height:250px;
    width:80%;
}
</style>
