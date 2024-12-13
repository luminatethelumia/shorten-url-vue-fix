<template>
  <div class="main">
    <div style="text-align:right;padding-top: 15px;padding-right: 10px;">
      <button @click="drawerVisible = true" class="toggle"><i class="las la-bars"></i>&#8942;&#8942;&#8942; Show URLs</button>
    </div>
    <div class="right-drawer"
    :style="{
    width: drawerVisible? '40vw' : '0',
    paddingLeft: drawerVisible? '0' : '0',
    opacity: drawerVisible? '0.9': '0'
    }"
    >
    <div class="drawer-mask-container">
      <span class="recent-url-text">Your recent url</span>
      <button class="close" @click="drawerVisible = false">
        &#9587;
      </button>
    </div>


    <div class="recent-url-list">
      <!-- <div style="display: flex; justify-content: center;"> -->
      <div class="total-url" style="gap: 10px; display: flex; justify-content: center; align-items: center; flex-basis: auto; width: 100%;text-align: center;margin-top: 10px;font-weight: bold;margin-bottom: 0;" ><p>Total URL: {{ urlList.length }} </p>
        <img v-if="isLoading" src="../assets/loading.gif" style="height: 20px; width: 20px ;" alt="">
      </div>
        
      <!-- </div> -->

      <UrlRecentList 
      v-for="url in urlList"
      :key="url.id"
      :qrbase64="url.qrImage"
      :shorturl="url.shortUrl"
      :longurl="url.url"
      :code="url.code"
      :apiurl="apiUrlTest"
      :date="url.createdAt"
      @refresh="getDataTime"
      >

      </UrlRecentList>

      <!-- <div v-for="(url, index) in urlList" :key="url.id" class="url-created">
        <input v-model="url.shortUrl" class="short-url" :disabled="!isEditing[index]" :class="{view: !isEditing[index]}">
        <input v-model="url.url" class="long-url" :disabled="!isEditing[index]" :class="{view: !isEditing}[index]">
        <div class="url-btn-list">

          <button class="url-btn-edit" @click="isEditing[index] = !isEditing[index]" v-if="!isEditing[index]">Edit</button>
          <button class="url-btn-edit" @click="save" v-else-if="isEditing[index]">Save</button>
          <button class="url-btn-delete" v-if="isEditing[index]" @click="cancel()">Cancel</button> -->

          <!-- <button class="url-btn-qr" v-if="!isEditing[index]">QR</button> -->

          <!-- <button class="url-btn-delete" v-if="!isEditing[index]">Delete</button> -->
        <!-- </div> -->
      <!-- </div> -->
      <!-- <div class="url-created">test2</div> -->

    </div>

    </div>
    <div class="drawer-mask"
    :style="{
      width: drawerVisible? '100vw': '0',
      opacity: drawerVisible? '0.6' : '0'
    }"
    @click="drawerVisible = false"></div>




    <div class="center">
    
        <form id="submit-url" @submit.prevent="">
          <div class="form-box">
          <div class="form-header">Shorten your URL</div>

            <input
            form="submit-url"
              type="text"
              class="short-url-input"
              placeholder="Enter a long link here"
              ref="my_input"
              required
            />

          <button form="submit-url" class="submit-url-btn" @click="getFormValues()" type="submit">Get shortened url</button>
          <div class="respone-succeed" v-if="alertMessage == 1">Url shortened! <br> <a :href="returnedUrl.url">{{ returnedUrl.url }}</a></div>
          <div class="respone-invalid" v-else-if="alertMessage == 2">Invalid Url!</div>
          <div class="respone-exist" v-else-if="alertMessage == 3">Url already exist!</div>

          

        </div>
        </form>
      
      </div>

    </div>

  
</template>


<script>
import UrlRecentList from './urlRecentList.vue';

export default {
  data() {
    return {
      drawerVisible: false,
      apiUrlTest: "https://localhost:8888",
      urlList: [],
      urlpost: "",
      alertMessage: 0, // 0 = none ,1 = url valid, 2 = invalid url, 3 = url exist
      returnedUrl: {},
      // isEditing: [false,false],
      urlLength: 0,
      isLoading: false
    };
  },
  components: {
UrlRecentList
  },
    methods: {

      getDataTime() {
        this.isLoading = true;
        setTimeout(() => {
          this.getDataTest()
          this.isLoading = false;
        }, 8000);
      },

    // GET URL FROM INPUT THEN DO POST
    getFormValues () {
        this.urlpost = ""
        this.urlpost = this.$refs.my_input.value
        if (this.urlpost == "") {
          console.log("no post url")
        }
        else {
          console.log(this.urlpost)
          this.postData(this.urlpost)
        }
    },
    editState() {
      for (let i = 0; i < this.urlLength; i++) {
          this.isEditing.push(false)
      }
      console.log(this.urlList.length)
    },
    // start editing an url
    startEdit() {

    },

    addKeyValue() {
      this.urlList.forEach((item) => {
        item.isEditing = false; // Add your key-value pair here
      })
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
        this.urlLength = finalRes.length;
        // this.editState()
        this.addKeyValue()

      } catch (error) {
        console.error(error.message);
      }
    },

    //POST
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
          else {
            const finalRes = await res.json();
            this.returnedUrl = finalRes
            if (finalRes == "Provided an invalid uri") {
              this.alertMessage = 2
            } else if (finalRes == "The uri is existed!") {
              this.alertMessage = 3
            } else {
              this.alertMessage = 1
            }

            // setTimeout(() => {
            //   console.log(finalRes)
            //   this.getDataTest()
            // }, 5000);
            this.getDataTime()
            console.log(finalRes)
            

          }


        } catch (error) {
          console.error(error.message);
        }
    },

    // edit shorten code

    //async putShortenCode(code) {
    //  try {
    //    const res = await fetch(this.apiUrlTest + "/UrlShorten/edit-shorten?code=" + code)
    //    if (!res.ok) {
    //      throw new Error(`Response status: ${res.status}`);
    //    }
    //    const finalRes = await res.json();
    //    console.log(finalRes)
    //  } catch (error) {
    //  }
    //},

  },
  mounted() {
    this.getDataTest()
    // this.editState()
    // console.log(this.isEditing)
    // this.cachedUser = Object.assign({}, this.user);
  },
  computed: {
  }
};
</script>


<style scoped>


/* .view {
  border-color: transparent;
  background-color: initial;
  color: initial
} */

.respone-invalid {
  text-align: center;
  font-size: 18px;
  font-weight: 400;
  color: #ffffff;
  border-radius: 5px;
  background-color: rgba(227, 71, 71, 0.5);
  width: 70%;
  min-height: 50px;
  align-content: center;
}

.respone-succeed {
  text-align: center;
  font-size: 18px;
  font-weight: 400;
  background-color: rgb(93, 194, 128, 0.5);
  color: white;
}

.respone-exist {
  text-align: center;
  font-size: 18px;
  font-weight: 400;
  background-color: rgba(227, 71, 71, 0.5);
  color: white;
}

/* .url-btn-edit {
  background-color: #b7e4c7;
  border: 0;
  font-size: 15px;
  width: 50px;
  height: 30px;
}

.url-btn-delete {
  background-color: #e1737e;
  border: 0;
  font-size: 15px;
  width: 50px;
  height: 30px;
}

.url-btn-qr {
  background-color: #0eaece;
  border: 0;
  font-size: 15px;
  width: 50px;
  height: 30px;
} */

/* .short-url {
  line-height: 25px;
  font-size: 25px;
  font-weight: bold;
} */

/* .long-url {
  line-height: 25px; */
  /* font-size: 10px; */
  /* font-weight: bold; */
/* } */

.respone-alert {
  opacity: 1;
  color: #dc3545;
  font-size: 25px;
}

.submit-url-btn {
  font-size: large;
  background-color: #1f8244;
  color: white;
  width: 80%;
  height: 50px;
  border: 0;
  border-radius: 20px;
}

.short-url-input {
  width: 80%;
    padding: 10px;
    font-size: 14px;
    border: 1.5px solid #ccc;
    border-radius: 20px;
    height: 30px;
    text-align: left;
}


.center {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
}

.form-box {
  background-color: white;
  border: 0px solid black;
  border-radius: 20px;
  width: 350px;
  height: 400px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  gap: 20px;
  /* background: rgb(162,233,120);
  background: linear-gradient(90deg, rgba(162,233,120,1) 0%, rgba(103,227,235,1) 100%);  */

}

.form-header {
  font-size: 35px;
  font-weight: bold;
  /* color: rgb(120, 229, 99); */
  color: darkblue;
}

/* .url-btn-list {
  display: flex;
  gap: 10px;
  justify-content: space-around;
} */

/* .url-created {
  display: flex;
  flex-direction: column;
  gap: 10px;
  padding: 10px;
} */

.recent-url-list {
  /* position: sticky; */
  /* display: flex; */
  /* padding: 15px; */
  /* padding-top: 100px; */
  /* justify-content: space-evenly; */
  /* align-items: center; */
  /* flex-direction: column; */
  /* width: 40vw; */
  overflow:hidden;
  overflow-y:scroll;
  /* max-height: calc(100vh - 9rem); */
  /* overflow-y: auto; */
  /* position: fixed; */
    /* width: 150px; */
    /* overflow-y: scroll; */
    /* bottom: 10px; */
    /* bottom: 0; */
    height: 800px;
    /* z-index: 201; */
  /* gap: 20px; */
}

/* .url-created {
  border: 1.5px solid grey;
  box-shadow: 0;
  text-shadow: 0; */
  /* width: 100%; */
  /* line-height: 20px; */
  /* padding-top: 20px; */
  /* margin: 20px 10px;
} */

.recent-url-text {
  font-size: 20px;
  font-weight: bold;
}

.toggle {
  border: 0;
  background: transparent;
  color: white;
  font-size: 20px;
}

.drawer-mask-container {
  position: sticky;
  display: flex;
  justify-content: space-between;
  align-items: start;
  border-bottom: 1px solid grey;
  line-height: 1.5em;
  padding: 15px;
}

.close {
  border: 0;
  background: transparent;
}

.right-drawer {
  position: absolute;
  top: 0;
  right: 0;
  width: 0; /* initially */
  /* overflow: hidden; */
  height: 100vh;
  padding-left: 0; /* initially */
  border-left: 1px solid whitesmoke;
  background: white;
  z-index: 200;
  transition: all 0.2s; /* for the animation */
  /* display: flex; */
  flex-direction: column;
  /* gap: 100px; */
  opacity: 0;
}

.drawer-mask {
  position: absolute;
  left: 0;
  top: 0;
  width: 0; /* initially */
  height: 100vh;
  background: #000;
  opacity: 0.3;
  z-index: 199;
}

</style>

