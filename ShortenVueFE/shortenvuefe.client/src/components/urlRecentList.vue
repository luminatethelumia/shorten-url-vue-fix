<template>
          <div v-if="!isDeleted" class="url-created">
        <!-- <div class="short-url-container"> -->
            <!-- <div class="short-url">{{ localhost }}</div> -->
            <!-- <input ref="first-url" :value="code"  class="code-url" :disabled="!isEditing" :class="{view: !isEditing}"> -->

        <!-- </div> -->
        <input ref="first-url" :value="datashorturl"  class="short-url" :disabled="!isEditing" :class="{view: !isEditing}">

        <input ref="last-url" :value="datalongurl" class="long-url"  :disabled="!isEditing" :class="{view: !isEditing}">

        <div >Created at: {{ formatDate }}</div>

    
        <div class="url-btn-list">

          <button class="url-btn-edit" @click="edit" v-if="!isEditing">Edit</button>
          <button class="url-btn-edit" @click="save" v-else-if="isEditing">Save</button>
          <button class="url-btn-delete" v-if="isEditing" @click="cancel">Cancel</button>

          <button @click="qrVisible = !qrVisible" class="url-btn-qr" v-if="!isEditing">QR</button>

          <button class="url-btn-delete" @click="deleteUrl();emitrefresh()" v-if="!isEditing">Delete</button>
        </div>
        <div v-if="qrVisible" :style="{backgroundImage: qrimg}" class="qr-code"></div>
        <div @click="qrVisible = false" v-if="qrVisible" class="qr-code-mask"></div>
        <div ></div>
        <div class="al"></div>
      </div>
</template>

<script>
export default {
data() {
    return {
        dateCreated: "",
        isEditing: false,
        localhost: "https://localhost:3000/api/",
        // tempUrL: "",
        isDeleted: false,
        newLongUrl: "",
        datashorturl: "",
        datalongurl: "",
        datacode: "",
        qrimg: "",
        qrVisible: false,
        formatDate: '',
    }
},
props: {
shorturl: {
    type: String,
    required: true
},
longurl: {
    type:String,
    required: true
},
apiurl : {
    type: String,
    required:true
},
code: {
    type:String,
    required:true
},
date: {
    type:String,
    required:true
},
qrbase64: {
  type:String,
  required:true
}
},

created(){
this.datashorturl = this.shorturl
this.datalongurl = this.longurl
this.datacode = this.code
this.qrimg = `url("data:image/jpg;base64,${this.qrbase64}")`
this.returnDate()
},

methods: {
    emitrefresh () {
        this.$emit('refresh')
    },

    returnDate() {
      
    const localDate = new Date(this.date)

    const formattedDate = localDate.toLocaleDateString('vi-VN', {
                                                    year: 'numeric',
                                                    month: 'short',
                                                    day: '2-digit',
                                                    hour: '2-digit',
                                                    minute: '2-digit'
                                                })

    this.formatDate = formattedDate

    },

    save() {
      const newcode = this.$refs['first-url'].value
      this.isEditing = !this.isEditing
      try {
        if (this.datashorturl == this.$refs['first-url'].value) {
        // this.datacode = newcode
        this.datashorturl = this.localhost + this.$refs['first-url'].value
          console.log('same url no put')
        }
        
      else {
        this.datashorturl = this.localhost + this.datacode
        console.log('send put')
        this.putShortenCode(newcode)
        // this.datacode = newcode
        // this.datashorturl = this.localhost + this.datacode

      }
      } catch (error) {
        // this.datashorturl = this.localhost
        console.log('save-post error')
      }

      try {
        if (this.datalongurl == this.$refs['last-url'].value)
          console.log('same origin url no put')

        else {

          this.datalongurl = this.$refs['last-url'].value
          console.log('send origin url put')

          setTimeout(() => {
            this.editOriginCode(newcode)
          }, 3000);

        }
      } catch (error) {
        console.log('edit post error')
      }


    },
    edit() {
        this.isEditing = !this.isEditing
        // this.tempUrL = this.shorturl
        this.datashorturl = this.datacode
        console.log(this.datashorturl)
    },
    cancel() {
        this.isEditing = false
        this.datashorturl = this.localhost + this.datashorturl
    },
  deleteUrl() {
    if (window.confirm('Are you sure you want to delete this?')) {
      this.isDeleted = true
      this.deleteShortenCode(this.datashorturl)
    }
    },

    // EDIT SHORT CODE

    async putShortenCode(newcode) {
      try {
        const res = await fetch(this.apiurl + "/UrlShorten/edit-shorten?code=" + newcode, {
            method: "PUT",
            headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
            },
            body: JSON.stringify({ url: this.datashorturl}),
        })
        if (!res.ok) {
          throw new Error(`Response status: ${res.status}`);
        }
        const finalRes = await res.json();
        this.datashorturl = finalRes.url
        this.datacode = newcode
        console.log(finalRes)
      } catch (error) {
        console.log(error)
      }
    },

    // DELETE
    async deleteShortenCode(urldelete) {
      try {
        const res = await fetch(this.apiurl + "/UrlShorten/delete-shorten", {
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

        //Edit Origin
        async editOriginCode(code) {
      try {
        const res = await fetch(`${this.apiurl}/UrlShorten/edit-origin?code=${code}`, {
          method: "PUT",
          headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
          },
          body: JSON.stringify({ url: this.datalongurl })
        });
        const finalRes = await res.json();
        console.log(finalRes);
      } catch (error) {
        console.error('Error:', error.message);
      }
    }

},




}

</script>

<style scoped>



.qr-code-mask{
  position: absolute;
  z-index: 209;
  width: 100%;
  background-color: grey;
  opacity: 0.2;
  height: 100%;
  top: 0;
  bottom: 0;
  left: 0;
  right: 0;
}

.qr-code {
  background-image: url('../assets/dog.jpeg');
  background-size: cover;
  background-position: center;
  position: absolute;
  z-index: 210;
  width: 100px;
  height: 100px;
  border: 1px solid black;
  right: 0;
  left: 0;
  top: 0;
  bottom: 0;
  margin: auto;

}

.code-url {
    display: inline-block;
line-height: 25px;
  font-size: 25px;
  font-weight: bold;
  width: 150px;
}

/* .short-url-container {
    display: block;
width: 100%;
} */

.view {
    /* display: inline-block; */
  border-color: transparent;
  background-color: initial;
  color: initial;
  /* width: 100%; */
}

/* .view .short-url {
    display: inline-block;
  border-color: transparent;
  background-color: initial;
  color: initial;
  width: 356px;
} */

.url-btn-list {
  display: flex;
  gap: 10px;
  justify-content: space-around;
}

.url-created {
  position: relative;
  display: flex;
  flex-direction: column;
  gap: 10px;
  padding: 10px;
  border: 1.5px solid grey;
  box-shadow: 0;
  text-shadow: 0;
  /* width: 100%; */
  line-height: 20px;
  /* padding-top: 20px; */
  margin: 20px 10px;
}

.short-url {
  line-height: 25px;
  font-size: 25px;
  font-weight: bold;
  /* display: inline-block; */
  /* width: 356px; */
  /* width: 100%; */

  /* flex-grow: 2; */
}

.long-url {
  line-height: 25px;
  /* font-size: 10px; */
  /* font-weight: bold; */
}

.url-btn-edit {
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
}

</style>
