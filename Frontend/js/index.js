
var user = {}
var scrollactive = false
var token

var zindex = 100;

async function fetchData(url,method = 'GET', options = {}) {
    try {
        let fetchBody
        if(method==='GET'){
            fetchBody = {
                method: method,
                headers:{
                    'Authorization':'Bearer '+token,
                    'Content-Type':'application/json'
                }
            }
        }
        else{
            fetchBody = {
                method: method,
                headers:{
                    'Authorization':'Bearer '+token,
                    'Content-Type':'application/json'
                },
                body:JSON.stringify(options)
            }
        }
        
      const response = await fetch('https://localhost:44300/api/'+url, fetchBody);
      
      if (!response.ok) {
        throw new Error(`Hiba: ${response.status} - ${response.statusText}`);
      }
      const text = await response.text();
      const data = text ? JSON.parse(text) : null;
      return data;
    } catch (error) {
      console.error('Fetch hiba:', error.message);
      return null;
    }
}

function getDate(){
    const now = new Date();

    const year = now.getFullYear();
    const month = String(now.getMonth() + 1).padStart(2, '0');
    const day = String(now.getDate()).padStart(2, '0');
    const hours = String(now.getHours()).padStart(2, '0');
    const minutes = String(now.getMinutes()).padStart(2, '0');

    const formatted = `${year}-${month}-${day} ${hours}:${minutes}`;
    return formatted;
}


document.addEventListener('DOMContentLoaded', async () => {
    try {
        const userData = localStorage.getItem('jobsearchuser') || sessionStorage.getItem('jobsearchuser');

        if (!userData) {
            // Nincs bejelentkezett felhasználó, átirányítás a login oldalra
            window.location.href = './login.html';
        } else {
            // Van bejelentkezett felhasználó, meghívhatjuk az API-t
            let userd = JSON.parse(userData);
    
            // Példa API-hívás (fetch-csel)
            let userf = await fetch('https://localhost:44300/api/felhasznalo/login/'+userd.email+"/"+userd.password, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
            })
            .then(response => {
                if (!response.ok) {
                    // Hibás azonosító, vissza a login oldalra
                    window.location.href = './login.html';
                }
                return response.json();
            })
            user = userf
            let resp = await fetch('https://localhost:44300/api/token', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({Email: userd.email, Jelszo: userd.password}),
            })
            token = await resp.json()
            
        }
        const ico = document.getElementById("usericon")
        if(user.profilKep!=null){
            ico.src = `https://jannn1.hu/${user.profilKep}`    
        }
        console.log("Autómatikus bejelentkezés sikeres!");
    } catch (error) {
        console.error("Hiba történt az Autómatikus bejelentkezéssel:", error);
    }

    const ignoreKeys = ["feladatok", "ertekelesek", "jelentkezesek", "mentesek"];

    let foundNull = false;

    for (const key in user) {
        if (user[key] === null && !ignoreKeys.includes(key)) {
            foundNull = true;
            break;
        }
    }

    if (foundNull) {
        document.getElementById("userdataalert").style.display = "flex"
    }else{
        document.getElementById("userdataalert").style.display = "none"
    }

    const alertappdata = await fetchData(`jelentkezesek/${user.user_Id}`)
    const alertapp= alertappdata.some(item =>(item.section=="elfogadva" || item.section=="elutasítva") && item.latta_e === 0)
    if(alertapp){
        document.getElementById("alertapp").style.display = "flex"
    }else{
        document.getElementById("alertapp").style.display = "none"
    }

    const alertmyjobapp = await fetchData(`jelentkezesek/sajatmunkak/${user.user_Id}`)
    
    if(alertmyjobapp==0){
        document.getElementById("myjobsalert").style.display = "flex"
    }else{
        document.getElementById("myjobsalert").style.display = "none"
    }


    if(foundNull || alertapp || alertmyjobapp==0){
        document.getElementById("notification").style.display = "flex"
    }else{
        document.getElementById("notification").style.display = "none"

    }

    document.getElementById("navico").addEventListener("click", function() {
        open();
    })

    document.getElementById("home").addEventListener("click",function(){
        //fooldal megjelenitése
        showhomepage(true);
        showjobscontainer(false);
        navigateToTop();
    
    })

    document.getElementById("jobs").addEventListener("click",function(){
        showhomepage(false);
        showjobscontainer(true);
        navigateToTop();
        document.getElementById("containerheader").style.display="flex"
        clearsearch();
    })
    
    document.getElementById("savedjobs").addEventListener("click",async function(){
        showhomepage(false);
        showjobscontainer(true);
        navigateToTop();
        document.getElementById("containerheader").style.display="none"

        const jobs = await fetchData(`munkakmentette/${user.user_Id}`)
        const mentettElemek = jobs.filter(elem => elem.mentes === true);
        loadjob(mentettElemek)
    
    })

    document.getElementById("mainjobs").addEventListener("click",function(){
        showhomepage(false);
        showjobscontainer(true);
        navigateToTop();
        document.getElementById("containerheader").style.display="flex"
        clearsearch();
    })
    
    document.getElementById("mainnewjob").addEventListener("click",function(){
        jobviewopen();
    })
    
    
    document.getElementById("viewalljobs").addEventListener("click",function(){
        showhomepage(false);
        showjobscontainer(true);
        navigateToTop();
        document.getElementById("containerheader").style.display="flex"
        clearsearch();
    })

    
    selectContainer = document.getElementById("groupselect");
    dropdown = document.getElementById("dropdowngroup");
    selectedItems = document.getElementById("selectedgroup");

    selectContainer.addEventListener("click", () => {
        dropdown.style.display = dropdown.style.display === "block" ? "none" : "block";
    });
    
    dropdown.addEventListener("click", (e) => {
        if (e.target.tagName === "INPUT") {
            updateSelectedItems();
        }
    });


    document.addEventListener("click", (e) => {
        if (!selectContainer.contains(e.target)) {
            dropdown.style.display = "none";
        }
    });

    selectContainer2 = document.getElementById("locationselect");
    dropdown2 = document.getElementById("dropdownlocation");
    selectedItems2 = document.getElementById("selectedlocation");

    selectContainer2.addEventListener("click", () => {
        dropdown2.style.display = dropdown2.style.display === "block" ? "none" : "block";
        jobsload();
    });
    
    dropdown2.addEventListener("click", (e) => {
        if (e.target.tagName === "INPUT") {
            updateSelectedItemslocation();
        }
    });
    
    // Bezárás ha máshová kattintunk
    document.addEventListener("click", (e) => {

        if ((!selectContainer2.contains(e.target) && dropdown2.style.display==="block")) {
            dropdown2.style.display = "none";
            jobsload();
        }
    });

    document.addEventListener("click", (e) => {

        if ((!selectContainer.contains(e.target) && dropdown.style.display==="block")) {
            dropdown.style.display = "none";
            jobsload();
        }
    });

    document.getElementById("search").addEventListener("input",function(){
        jobsload();
    });

    document.getElementById("clearbtn").addEventListener("click",function(){
        clearsearch();
    })

    /*Panel */
    document.getElementById("navuser").addEventListener("click",function(){
        document.getElementById('navusermanage').classList.toggle('active');
    })

    document.getElementById("navusermanage").addEventListener("mouseleave",function(){
        document.getElementById('navusermanage').classList.toggle('active');
    })

    document.getElementById("userdata").addEventListener("click",function(){
        userpanelopen("Felhasználó adatok")
    })

    document.getElementById("logout").addEventListener("click",function(){
        showConfirm('Biztosan kijelentkezel?', function(igen) {
            if (igen) {
                //kijelentkezés
                localStorage.removeItem('jobsearchuser')
                sessionStorage.removeItem('jobsearchuser')
                window.location.href = './login.html';
            }
        });
        
    })

    document.getElementById("myjobs").addEventListener("click",function(){
        userpanelopen("Munkáim")
    })

    document.getElementById("jobapplications").addEventListener("click",function(){
        document.getElementById("application-panel").setAttribute("data-action","sent")
        applicationsOpen(true)
        
    })

        
    document.getElementById("newjob").addEventListener("click",function(){
        //ide
        jobviewopen()
    })

    document.getElementById("myprofile").addEventListener("click",function(){
        profileviewopen(user.user_Id)
        scrollactive = true;
        scroll(false)
    })

    document.getElementById("panelx").addEventListener("click",function(){
        showuserpanel(false)
        showblur(false)
        scroll(true);
    })

    document.getElementById("panelsearch").addEventListener("input",async function(){
        const myjobs = await fetchData(`sajatmunkak/${user.user_Id}`)
        const panel = document.getElementById("panelcontent");

        Array.from(panel.children).forEach(child => {
        if (!child.classList.contains("useredit")) {
            panel.removeChild(child);
        }
        });
        let kereses = document.getElementById("panelsearch").value
        let closedjob = document.getElementById("upanelcheck").checked
        myjobsload(myjobs,kereses,closedjob).forEach(job=>document.getElementById("panelcontent").appendChild((createjobcard(job,true))))
    
    })
    
    document.getElementById("upanelcheck").addEventListener("input",async function(){
        const myjobs = await fetchData(`sajatmunkak/${user.user_Id}`)
        const panel = document.getElementById("panelcontent");

        Array.from(panel.children).forEach(child => {
        if (!child.classList.contains("useredit")) {
            panel.removeChild(child);
        }
        });
        let kereses = document.getElementById("panelsearch").value
        let closedjob = document.getElementById("upanelcheck").checked
        myjobsload(myjobs,kereses,closedjob).forEach(job=>document.getElementById("panelcontent").appendChild((createjobcard(job,true))))
    })

    /* Profilkep feltoltes*/
    document.getElementById("profile-image").addEventListener('change', function(event) {
        const file = event.target.files[0];
    
        
        if (file) {
            // Ellenőrizzük, hogy kép típusú fájl lett kiválasztva-e
            if (file.type.startsWith('image/')) {
                if (file.size > 50 * 1024 * 1024) { // 5 MB-nál nagyobb fájl
                    document.getElementById("uicpreviewtext").style.display="block"                
                    alert('A fájl mérete túl nagy! Maximum 5 MB engedélyezett.');
                    return;
                }
    
                const reader = new FileReader();
                
                // Amikor a FileReader beolvasta a fájlt, beállítjuk az <img> src attribútumát
                reader.onload = function(e) {
                    const imgElement = document.getElementById('usericonpreview');
                    imgElement.src = e.target.result; // Ez lesz a kép adata (base64)
                    imgElement.style.display = 'block'; // Megjelenítjük a képet
                };
    
                // Olvassuk be a fájlt base64 formátumba
                reader.readAsDataURL(file);
    
                document.getElementById("uicpreviewtext").style.display="none"
    
            } else {
            document.getElementById("uicpreviewtext").style.display="block"
    
                alert('Csak kép típusú fájlokat tölts fel!');
            }
        } else {
            document.getElementById("uicpreviewtext").style.display="block"
            document.getElementById('usericonpreview').style.display = 'none';
        }
    });
    
    
    /*Profil adatok mentese */
    document.getElementById('savedatas').addEventListener('click', async function () {
        showConfirm('Biztosan kijelentkezel?', async function(igen) {
            if (igen) {
                const fileInput = document.getElementById('profile-image');
                const file = fileInput.files[0];
                let udata = {
                }
                udata = user

                const vname = document.getElementById("vname").value.trim();
                if (vname !== "") udata.vezNev = vname;
                
                const kname = document.getElementById("kname").value.trim();
                if (kname !== "") udata.kerNev = kname;
                
                const pw = document.getElementById("pw").value;
                const pw2 = document.getElementById("pw2").value;
                if (pw !== "" && pw === pw2) {
                    udata.jelszo = pw;
                }
                
                const phone = document.getElementById("phonenumber").value.trim();
                if (/^\+?[0-9]{7,15}$/.test(phone)) { // egyszerű nemzetközi szám formátum
                    udata.telefonszam = phone;
                }
                
                const birthdate = document.getElementById("birthdate").value;
                if (birthdate !== "") {
                    const date = new Date(birthdate);
                    const now = new Date();
                    if (!isNaN(date) && date < now) { // érvényes és múltbéli dátum
                        udata.szulDat = birthdate;
                    }
                }
                
                const bio = document.getElementById("bio").value.trim();
                if (bio !== "") udata.bio = bio;
                
                


                if (file) {
                    const formData = new FormData();
                    formData.append('image', file);
                
                    await fetch('https://jannn1.hu/profile_image_upload.php', {
                        method: 'POST',
                        body: formData
                    })
                    .then(response => response.json())
                    .then(data => {
                        if (data.success) {
                            alert('Sikeres feltöltés: ' + data.resizedPath);
                            udata.profilKep = data.resizedPath
                        } else {
                            alert('Hiba: ' + data.message);
                        }
                    })
                    .catch(error => {
                        console.error('Hiba történt:', error);
                        alert('Hiba történt a feltöltés során.');
                    });
                }

                

                let ered = await fetchData("felhasznalo/updatedata", "PATCH", udata)
                console.log(ered)
            }
        });
        
        
    });


    document.getElementById("jobviewx").addEventListener("click",function(){
        showjobfullview(false);
        
    })

    headerOptions = document.querySelectorAll('.header-option');
    // Header option click event
    headerOptions.forEach(option => {
        option.addEventListener('click', () => {
            // Remove active class from all options
            headerOptions.forEach(opt => opt.classList.remove('active'));
            
            // Add active class to clicked option
            option.classList.add('active');
    
            loadApplications();
        });
    });


    document.getElementById("applicationclose").addEventListener("click",function(){
        applicationsOpen(false)
    })


    document.getElementById("profileviewx").addEventListener("click",function(){
        profileview(false)
    })
    
    tabs = document.querySelectorAll('.pcard-tab');
    tabContents = document.querySelectorAll('.pcard-tab-content');

    // Tab váltás
    tabs.forEach(tab => {
        tab.addEventListener('click', () => {
            const targetId = tab.dataset.tab;
            const targetContent = document.getElementById(targetId);

            tabs.forEach(t => t.classList.remove('pcard-active'));
            tab.classList.add('pcard-active');

            tabContents.forEach(content => content.classList.remove('pcard-active'));
            if (targetContent) targetContent.classList.add('pcard-active');
        });
    });

    newRatingForm = document.getElementById('pcard-new-rating-form');
    starInputContainer = document.querySelector('.pcard-star-input-container');
    ratingStarsInput = starInputContainer ? starInputContainer.querySelectorAll('i') : [];
    hiddenScoreInput = document.getElementById('pcard-rating-score-hidden');


    document.getElementById('pcard-new-rating-form-btn').addEventListener('click',async function(event) {
        showConfirm('Biztosan folytatod?', async function(igen){
            if (igen) {
                event.preventDefault();
    
                const descriptionInput = document.getElementById('pcard-rating-description');
                const score = parseInt(hiddenScoreInput.value, 10);
            
                //kell felhasznalonev
                const username = `${user.vezNev} ${user.kerNev}`;
                const description = descriptionInput.value.trim();
                if (!username || isNaN(score) || score < 1 || score > 5 || !description) {
                    alert("Kérlek tölts ki minden mezőt, és adj meg egy 1-5 közötti értékelést.");
                    return;
                }
            
                const safeUsername = username.replace(/</g, "&lt;").replace(/>/g, "&gt;");
                const safeDescription = description.replace(/</g, "&lt;").replace(/>/g, "&gt;");
            
                const newRatingItem = document.createElement('div');
                newRatingItem.classList.add('pcard-rating-item');
                newRatingItem.innerHTML = `
                    <div class="pcard-rating-header">
                        <span class="pcard-username">${safeUsername}</span>
                        <span class="pcard-stars">${generateStarsHTML(score)}</span>
                    </div>
                    <p class="pcard-description">${safeDescription}</p>
                `;
                let ertekeles = {
                    ertekeles: score,
                    ertekelo_Id: user.user_Id,
                    ertekelt_Id: parseInt(document.getElementById("pcard-name").getAttribute("uid")),
                    comment: description,
                    erDatum: getDate(),
                }
                console.log(ertekeles)
                let ered = await fetchData("felhasznaloertekeles", "POST", ertekeles)
                console.log(ered)
            
                document.getElementById('pcard-ratings-list-container').appendChild(newRatingItem);
                
                descriptionInput.value = ""; // Reset the description input
            }
        });
        
    });

    if (starInputContainer && hiddenScoreInput) {
        let currentRating = 0;

        function updateStarDisplay(rating) {
            ratingStarsInput.forEach(star => {
                const starValue = parseInt(star.dataset.value, 10);
                star.style.color = starValue <= rating
                    ? getComputedStyle(document.documentElement).getPropertyValue('--pcard-star-color')
                    : getComputedStyle(document.documentElement).getPropertyValue('--pcard-grey-color');
            });
        }

        updateStarDisplay(0);

        ratingStarsInput.forEach(star => {
            const starValue = parseInt(star.dataset.value, 10);
            const hoverColor = getComputedStyle(document.documentElement).getPropertyValue('--pcard-star-color-hover');
            const greyColor = getComputedStyle(document.documentElement).getPropertyValue('--pcard-grey-color');

            star.addEventListener('mouseenter', () => {
                ratingStarsInput.forEach(s => {
                    const val = parseInt(s.dataset.value, 10);
                    s.style.color = val <= starValue ? hoverColor : greyColor;
                });
            });

            star.addEventListener('click', () => {
                currentRating = starValue;
                hiddenScoreInput.value = currentRating;
                updateStarDisplay(currentRating);
            });
        });

        starInputContainer.addEventListener('mouseleave', () => {
            updateStarDisplay(currentRating);
        });

        newRatingForm.addEventListener('reset', () => {
            currentRating = 0;
            hiddenScoreInput.value = "";
            updateStarDisplay(currentRating);
        });
    }
    jobsload()
    loadselects();

});



function open(){
    document.getElementById("nav").classList.toggle("open");
}

/*Noscroll */

function scroll(value){

    if(value==false){
        document.body.classList.add("no-scroll")
    }else{
        document.body.classList.remove("no-scroll")
    }    
}

/*Blur */
function showblur(show){
    if(show){
        scroll(false)
        document.getElementById("blur-background").classList.add("blur-background")
    }else{
        scroll(true)
        document.getElementById("blur-background").classList.remove("blur-background")
    }
}

/*Userpanel */
function showuserpanel(show){
    if(show){
        showblur(true)
        document.getElementById("userpanel").classList.add("show")
        document.getElementById("userpanel").style.zIndex = zindex;
        zindex++;
    }else{
        //showblur(false)
        upanelcheck()
        document.getElementById("userpanel").classList.remove("show")
    }
}

function upanelcheck(){
    if(!document.getElementById("userpanel").classList.contains("show")){
        showblur(false)
    }
}

/*jobfullview */
function showjobfullview(show){
    if(show){
        showblur(true)
        document.getElementById("jobfullview").classList.add("show")
        document.getElementById("jobviewx").style.display = "block"
        document.getElementById("jobfullview").style.zIndex = zindex;
        zindex++;
        document.getElementById("jobviewx").style.zIndex = zindex;
        zindex++;
    }else{
        upanelcheck()
        document.getElementById("jobfullview").classList.remove("show")
        document.getElementById("jobviewx").style.display = "none"
    }
}

/*clearbtn */
function showclearbtn(show){
    if(show){
        document.getElementById("clearbtn").classList.add("show")
    }else{
        document.getElementById("clearbtn").classList.remove("show")
    }
}

function showhomepage(show){
    if(show){
        document.getElementById("homepage").classList.add("show")
    }else{
        document.getElementById("homepage").classList.remove("show")
    }
}

function showjobscontainer(show){
    if(show){
        document.getElementById("container").classList.add("show")
    }else{
        document.getElementById("container").classList.remove("show")
    }
}


/*HomePage */
function navigateToTop(){
    window.scrollTo({
        top: 0,
        behavior: 'smooth'
    });
}






async function loadselects(){
    
    //getgroups

    const gdrop = document.getElementById("dropdowngroup")    

    const groups = await fetchData("Kategoria/")

    groups.forEach(group => {
        const label = document.createElement('label');
        const input = document.createElement('input');

        input.type = 'checkbox';
        input.value = group.kat_id;
        label.appendChild(input);
        label.appendChild(document.createTextNode(` ${group.katnev}`));

        gdrop.appendChild(label);
        
    });

    //getlocations

    const ldrop = document.getElementById("dropdownlocation")

    const locations = await fetchData("Feladat/Helyszinek")
    
    locations.forEach(location => {
        const label = document.createElement('label');
        const input = document.createElement('input');

        input.type = 'checkbox';
        input.value = location
        label.appendChild(input);
        label.appendChild(document.createTextNode(` ${location}`));

        ldrop.appendChild(label);
        
    });    
}



var selectContainer
var dropdown 
var selectedItems



function updateSelectedItems() {
    const selectedCheckboxes = document.querySelectorAll("#dropdowngroup input:checked");
    const values = Array.from(selectedCheckboxes).map(checkbox => checkbox.parentNode.textContent.trim());
    selectedItems.textContent = values.length > 0 ? values.join(", ") : "Válassz...";
}

// Bezárás ha máshová kattintunk


var selectContainer2 
var dropdown2 
var selectedItems2


function updateSelectedItemslocation() {
    const selectedCheckboxes = document.querySelectorAll("#dropdownlocation input:checked");
    const values = Array.from(selectedCheckboxes).map(checkbox => checkbox.parentNode.textContent.trim());
    selectedItems2.textContent = values.length > 0 ? values.join(", ") : "Válassz...";
}




//torolni a többi kijelölest is
//kereses törlése
function clearsearch(){
    document.getElementById("search").value = "";
    const selectedCheckboxloc = document.querySelectorAll("#dropdownlocation input:checked");
    const selectedCheckboxgro = document.querySelectorAll("#dropdowngroup input:checked");
    selectedCheckboxloc.forEach(checkbox => {
        checkbox.checked = false;
    });
    selectedCheckboxgro.forEach(checkbox => {
        checkbox.checked = false;
    });
    updateSelectedItems();
    updateSelectedItemslocation();
    jobsload();
}


//jobload

function myjobsload(jobs, keres = "", closed) {
    return jobs.filter(data => {
        const cimMatch = keres === "" || data.cim.toLowerCase().includes(keres.toLowerCase());
        const isClosed = data.statusz === "lezárt";
        return cimMatch && (closed || !isClosed);
    });
}

async function userpanelopen(title){
    document.getElementById("panelheader").innerText = title
    const panelbody = document.getElementById("panelcontent")
    Array.from(panelbody.children).forEach(element => {
        element.style.display = "none";
    });
    document.getElementById("upanelsearchcontainer").style.display="none";
    document.getElementById("savedatas").style.display="none";
    document.getElementById("upcheckcontainer").style.display="none";
    if(title=="Felhasználó adatok"){
        document.getElementById("savedatas").style.display="block";
        Array.from(document.getElementsByClassName("useredit")).forEach(element => {
            element.style.display = "block";
        });

        document.getElementById("vname").value=user.vezNev;
        
        document.getElementById("kname").value=user.kerNev;

        document.getElementById("phonenumber").value=user.telefonszam

        document.getElementById("birthdate").value=user.szulDat;

        document.getElementById("bio").value=user.bio;

    }
    else if(title=="Munkáim"){
        document.getElementById("upanelsearchcontainer").style.display="flex";
        document.getElementById("upcheckcontainer").style.display="flex";
        Array.from(document.getElementsByClassName("userjobs")).forEach(element => {
            element.style.display = "block";
        });
        
        const myjobs = await fetchData(`sajatmunkak/${user.user_Id}`)
        let kereses = document.getElementById("panelsearch").value
        let closedjob = document.getElementById("upanelcheck").checked
        myjobsload(myjobs,kereses,closedjob).forEach(job=>panelbody.appendChild((createjobcard(job,true))))
    }
    else{
        Array.from(document.getElementsByClassName("useredit")).forEach(element => {
            element.style.display = "block";
        });
    }

    showuserpanel(true)    
    //document.getElementById("userpanel").classList.toggle("show");
    //document.getElementById("blur-background").classList.toggle("blur-background");
    scroll(false);
}





function uploadImage() {
    const fileInput = document.getElementById('profile-image');
    const file = fileInput.files[0];

    if (!file) {
        alert('Kérlek válassz egy fájlt!');
        return;
    }

    const formData = new FormData();
    formData.append('image', file);

    fetch('https://jannn1.hu/profile_image_upload.php', {
        method: 'POST',
        body: formData
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            alert('Sikeres feltöltés: ' + data.resizedPath);
        } else {
            alert('Hiba: ' + data.message);
        }
    })
    .catch(error => {
        console.error('Hiba történt:', error);
        alert('Hiba történt a feltöltés során.');
    });
}



//jobsload
async function jobsload(){
    //jobs

    let jobs = await fetchData("munkakmentette/"+user.user_Id)
    //group csoportositasa

    const selectedCheckboxloc = document.querySelectorAll("#dropdownlocation input:checked");
    const selectedCheckboxgro = document.querySelectorAll("#dropdowngroup input:checked");

    const selectedLocations = Array.from(selectedCheckboxloc).map(checkbox => checkbox.value); 
    const selectedGroups = Array.from(selectedCheckboxgro).map(checkbox => checkbox.value); 
    const search = document.getElementById("search").value.trim().toLowerCase(); 

    //clearbtn megjelenitese
    showclearbtn((search!=="" || selectedLocations.length!==0 || selectedGroups.length!== 0))
    

    console.log(selectedGroups);
    console.log(jobs)
    
    const szurtMunkak = await jobs.filter((munka) => {
        const helyszinTalalat =
            selectedLocations.length === 0 || selectedLocations.includes(munka.helyszin);

        const kategoriaTalalat =
            selectedGroups.length === 0 ||
            (munka.feladatKategoriak &&
                selectedGroups.some((group) => munka.feladatKategoriak.includes(group)));

        const keresesTalalat =
            search === "" ||
            (munka.cim && munka.cim.toLowerCase().includes(search)) ||
            (munka.leiras && munka.leiras.toLowerCase().includes(search));
        // A keresésnek külön szűrőként kell működnie az összes feltétellel együtt
        return helyszinTalalat && kategoriaTalalat && keresesTalalat;
    });
    document.getElementById("job-count").innerText = `${szurtMunkak.length} találat`
    loadjob(szurtMunkak)

    }

//fejlecet meg kell hagyni es a többit betolteni
function loadjob(newitems){
    const container = document.getElementById("container")
    const items = container.querySelectorAll(".containeritem")
    items.forEach(item => item.remove())
    
    newitems.forEach(i=> document.getElementById("container").appendChild(createjobcard(i)))

}



/* Munka kartyak letrehozasa*/
//adatok,az edit panel resze latszodjon e
function createjobcard(data,edit) {
    
    edit = arguments.length < 2 ? false : edit;

    
    const container = document.createElement('div');
    container.classList.add('containeritem');
    console.log(data)
    if(data.statusz=="lezárt"){
        container.classList.add("closedjob")
    }

    const jobCard = document.createElement('div');
    jobCard.classList.add('jobcard');

    const header = document.createElement('div');
    header.classList.add('jobcard-header');

    const span = document.createElement('span');

    //ber
    span.textContent = `${data.fizetes} Ft`;

    console.log(data)
    const iconContainer = document.createElement('div');

    const svg1 = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
    svg1.setAttribute('class', 'saveicons');
    svg1.setAttribute('viewBox', '0 0 24 24');
    svg1.setAttribute('fill', 'none');
    svg1.setAttribute('xmlns', 'http://www.w3.org/2000/svg');
    svg1.setAttribute('stroke', '#0084ff');
    svg1.innerHTML = `<path d="M6.85665 2.30447C8.2922 2.16896 10.3981 2 12 2C13.6019 2 15.7078 2.16896 17.1433 2.30447C18.4976 2.4323 19.549 3.51015 19.6498 4.85178C19.7924 6.74918 20 9.90785 20 12.2367C20 14.022 19.8781 16.2915 19.7575 18.1035C19.697 19.0119 19.6365 19.8097 19.5911 20.3806C19.5685 20.6661 19.5496 20.8949 19.5363 21.0526L19.5209 21.234L19.5154 21.2966L19.5153 21.2976C19.5153 21.2977 19.5153 21.2977 18.7441 21.2308L19.5153 21.2976C19.4927 21.5553 19.3412 21.7845 19.1122 21.9075C18.8831 22.0305 18.6072 22.0309 18.3779 21.9085L12.1221 18.5713C12.0458 18.5307 11.9542 18.5307 11.8779 18.5713L5.62213 21.9085C5.39277 22.0309 5.11687 22.0305 4.88784 21.9075C4.65881 21.7845 4.50732 21.5554 4.48466 21.2977L5.25591 21.2308C4.48466 21.2977 4.48467 21.2978 4.48466 21.2977L4.47913 21.234L4.46371 21.0526C4.45045 20.8949 4.43154 20.6661 4.40885 20.3806C4.3635 19.8097 4.30303 19.0119 4.24255 18.1035C4.12191 16.2915 4 14.022 4 12.2367C4 9.90785 4.20763 6.74918 4.3502 4.85178C4.45102 3.51015 5.50236 2.4323 6.85665 2.30447ZM5.93179 19.9971L11.1455 17.2159C11.6791 16.9312 12.3209 16.9312 12.8545 17.2159L18.0682 19.9971C18.1101 19.4598 18.1613 18.7707 18.2124 18.0019C18.3327 16.1962 18.4516 13.9687 18.4516 12.2367C18.4516 9.97099 18.2482 6.86326 18.1057 4.96632C18.0606 4.366 17.5938 3.89237 16.9969 3.83603C15.5651 3.70088 13.5225 3.53846 12 3.53846C10.4775 3.53846 8.43487 3.70088 7.00309 3.83603C6.40624 3.89237 5.9394 4.366 5.89429 4.96632C5.75175 6.86326 5.54839 9.97099 5.54839 12.2367C5.54839 13.9687 5.66734 16.1962 5.78756 18.0019C5.83874 18.7707 5.88993 19.4598 5.93179 19.9971Z" fill="#0084ff"></path>`;
    
    const svg2 = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
    svg2.setAttribute('class', 'saveicons');
    svg2.setAttribute('viewBox', '0 0 24 24');
    svg2.setAttribute('fill', 'none');
    svg2.setAttribute('xmlns', 'http://www.w3.org/2000/svg');
    svg2.setAttribute('stroke', '#0084ff');
    svg2.innerHTML = `<path d="M20 12.2565C20 15.7437 19.4904 21.077 19.4904 21.2821C19.4904 21.4872 19.2866 21.7949 19.0828 21.8975C18.9809 22.0001 18.879 22.0001 18.6752 22.0001C18.5732 22.0001 18.4713 22.0001 18.2675 21.8975L12.051 18.6154C11.949 18.6154 11.8471 18.6154 11.8471 18.6154L5.63057 21.8975C5.42675 22.0001 5.12102 22.0001 4.9172 21.8975C4.71338 21.7949 4.50955 21.5898 4.50955 21.2821C4.50955 21.077 4 15.7437 4 12.2565C4.10191 10.0001 4.30573 7.0257 4.50955 4.87186C4.61146 3.53852 5.63057 2.51288 6.95541 2.30775C8.38217 2.20519 10.4204 2.00006 12.051 2.00006C13.6815 2.00006 15.7197 2.20519 17.1465 2.30775C18.4713 2.41032 19.4904 3.53852 19.5924 4.87186C19.7962 7.0257 20 10.0001 20 12.2565Z" fill="#0084FF"></path>`;

    if(!data.mentes){
        svg2.classList.add("hide")
    }else{
        svg1.classList.add("hide")
    }

    iconContainer.appendChild(svg1);
    iconContainer.appendChild(svg2);
    
    iconContainer.setAttribute("jobid",data.task_Id)
    iconContainer.addEventListener("click",async function (event) {
        // Az éppen meghívott iconContainer referenciája
        const currentContainer = event.currentTarget;
        console.log(currentContainer)
        // Az aktuális iconContainer-ben lévő összes SVG lekérése
        const svgs = currentContainer.querySelectorAll('svg');
        
        if (svgs.length === 2) {
            // Az SVG-k osztályainak váltása
            svgs[0].classList.toggle('hide');
            svgs[1].classList.toggle('hide');
        }
        let selectedjobid = currentContainer.getAttribute('jobid');
        //ide meg kell fetch ami lementi a valtoztatast
        if(svgs[0].classList.contains('hide')){
            await fetchData("mentes/","POST",{Task_Id: selectedjobid ,User_Id: user.user_Id})
            
        }else{
            await fetchData(`mentestorles/${user.user_Id}/${selectedjobid}`,"DELETE")
        }


    });
        
    header.appendChild(span);
    if(edit){
        const listbtn = document.createElement('button');
        listbtn.classList.add('jobcard-btn');    
        listbtn.textContent = 'Jelentkezések';
        listbtn.setAttribute("jobid",data.task_Id);
        
        listbtn.addEventListener("click",function(event){
            let selectedjob = event.currentTarget.getAttribute('jobid');
            document.getElementById("application-panel").setAttribute("data-action","received")
            document.getElementById("application-panel").setAttribute("jobid",selectedjob)
            //applicationpanel
            applicationsOpen(true)

            loadApplications()
            
        })

        if(data.ujjelentkezes){
            const listalert = document.createElement("div");
            listalert.classList.add('alert')
            listalert.textContent="!"
            listbtn.appendChild(listalert)
        }
        


        header.appendChild(listbtn)
    }else{
        header.appendChild(iconContainer);
    }
    

    const title = document.createElement('p');
    title.classList.add('jobcard-title');

    //munka neve
    title.textContent = data.cim;

    jobCard.appendChild(header);
    jobCard.appendChild(title);

    //jelentkezesek


    const footer = document.createElement('div');
    footer.classList.add('jobcard-footer');

    const summary = document.createElement('div');
    summary.classList.add('jobcard-summary');

    const location = document.createElement('p');
    location.classList.add('jobcard-location');

    //helyszin
    location.textContent = data.helyszin;

    summary.appendChild(location);
    footer.appendChild(summary);

    const button = document.createElement('button');
    button.classList.add('jobcard-btn');    
    button.textContent = 'Több információ';
    button.addEventListener("click",async function(event){
        jobviewopen(event)
    })
    
    
    button.setAttribute("jobid",data.task_Id);

    footer.appendChild(button);


    container.appendChild(jobCard);
    container.appendChild(footer);
    //munka lezárása
    if(edit){
        const buttonclose = document.createElement('button');
        buttonclose.classList.add('jobcard-btn');    
        buttonclose.textContent = 'Munka lezárása';
        buttonclose.setAttribute("jobid",data.task_Id);
        buttonclose.addEventListener("click", function(event){
            const targetevent = event
            showConfirm('Biztosan lezárod?', function(igen) {
                if (igen) {
                    jobStatusClose(targetevent)
                }
              });
            
        })
        container.appendChild(buttonclose)
    }
   return container
    
}

async function jobStatusClose(event){
    
    let jobid = event.currentTarget.getAttribute("jobid")
    let a = await fetchData(`Feladat/statusupdate/${jobid}/lezárt`,"PATCH")
    
}

//munka megtekintese

async function addCategoryToJob(){
    let jobid = document.getElementById("cataddbtn").getAttribute("jobid");
    let katname = document.getElementById("categoryadd").value;
    let res = await fetchData(`kategoria/ujkategoriahozzaadas/${jobid}/${katname}`,"POST");


    if(res!=null){
        let span = document.createElement("span")
        span.innerText=katname
        let x = document.createElement("div")
        x.innerText="X"
        x.setAttribute("katid",res.kat_id)
        x.setAttribute("jobid",jobid)
        x.addEventListener("click",async function(event){
            let katid = event.currentTarget.getAttribute("katid")
            let jobid = event.currentTarget.getAttribute("jobid")
            //torles fetch
            let element = event.currentTarget.parentElement
            
            await fetchData(`kategoria/kapcsolattorles/${jobid}/${katid}`,"DELETE");
            
            element.remove();

            
        })
        span.appendChild(x)
        
        document.getElementById("job-categorys").appendChild(span)

        document.getElementById("categoryadd").value=""
    }
}


async function jobviewopen(event){
    const jobview = document.getElementById("jobfullview")
    const upanel = document.getElementById("userpanel")
    let current
    let jobdata
    let newjob = false;
    //adatok lekerese ha nem újat hoz létre
    if(event!=null){
        jobdata = JSON.parse(event.currentTarget.getAttribute("jobid"));
        current = await fetchData(`getmunka/${jobdata}/${user.user_Id}`)
    }
    
    if(upanel.classList.contains("show") || event==null){            
        
        
        if(event==null){
            current = {
                cim:"",
                leiras:"",
                posztDatum: getDate(),
                helyszin:"", 
                fizetes:"",
                idotartam:"",
                hatarido:"",
                statusz:"",
                felhasznalo:{
                    vezNev:"",
                    kerNev:""
                },
            }
            newjob= true
        }
        current.hatarido = current.hatarido.replace(".","-")
        current.hatarido = current.hatarido.replace(".","-")
        current.hatarido = current.hatarido.replaceAll(" ","")
        current.hatarido = current.hatarido.substring(0,10)
        console.log(current)
        jobview.innerHTML = await `
            <div class="content">
                <div class="job-header-container">
                    <div class="job-header">
                        <h1>Név:<input type="text" id="job-titlename" value="${current.cim}" jobstatus="${current.statusz}"></h1>
                        
                    </div>
                    <div class="job-meta" id="job-categorys">
                        
                    </div>
                    <div> 
                        <span>${newjob==true ? "Csak mentés után adhatsz hozzá kategóriát" : "Új kategória hozzáadása:"}</span>
                        <input type="text" id="categoryadd" ${newjob==true ? "disabled" : ""}>
                        <input type="button" id="cataddbtn" value="Hozzáad" jobid="${jobdata}" ${newjob==true ? "disabled" : ""}>
                    </div>
                </div>
                <div class="job-description-container">
                    <h2>Munka leírása:</h2>
                    <p>
                        <textarea id="job-description">${current.leiras}</textarea>
                    </p>
                </div>
            </div>
            <div class="sidebar">
                <div class="job-details-container">
                    <h2>Munka részletei</h2>
                    <p><strong>Meghirdetés dátuma:</strong><span id="job-createdate">${current.posztDatum}</span></p>
                    <p><strong>Helyszín:</strong><span><input type="text" id="job-location" value="${current.helyszin}"></span></p>
                    <p><strong>Fizetés:</strong><span><input type="text" id="job-salary" value="${current.fizetes}">Ft</span></p>
                    <p><strong>Időtartam:</strong><span><input type="text" id="job-time" value="${current.idotartam}"></span></p>
                    <p><strong>Határidő:</strong><span><input type="date" id="job-date" value="${current.hatarido}"></span></p>
                    <p><strong>Hirdető:</strong><span id="jobowner">${current.felhasznalo.vezNev} ${current.felhasznalo.kerNev}</span></p>
                </div>
                <div class="apply-section-container">
                ${current.statusz!="lezárt"
                    ? `<h2>Változtatások mentése</h2>
                        <button class="apply-button" id="jobactionbutton" jobid="${jobdata}">Mentés most →</button>
                        ${newjob!=true ? `<button class="apply-button" id="jobdelete" jobid="${jobdata}">Zárja le most →</button>`: ""}`
                    : `<h2>Munka lezárva</h2>`
                }
                </div>
            </div>
        `;
        if(current.statusz!="lezárt"){
            //kategoria hozzaad
            document.getElementById("cataddbtn").addEventListener("click", function(){
                addCategoryToJob()
            })
            //kategoria input enterre is hozzadja
            document.getElementById("categoryadd").addEventListener("keydown",function(){
                if (event.key === 'Enter') {
                    
                    addCategoryToJob()
                }
            })

            //munka adatainak frissitese
            document.getElementById("jobactionbutton").addEventListener("click",async function(event){
                let target = event.currentTarget
                let jobid = target.getAttribute("jobid")
                showConfirm('Biztosan mented?', async function(igen) {
                    if (igen) {
                        if(jobid=="undefined" || jobid==null){
                            let data = {
                                Statusz: document.getElementById("job-titlename").getAttribute("jobstatus"),
                                Helyszin: document.getElementById("job-location").value,
                                Cim: document.getElementById("job-titlename").value,
                                Hatarido: document.getElementById("job-date").value,
                                Leiras: document.getElementById("job-description").value,
                                Idotartam: document.getElementById("job-time").value,
                                Fizetes: document.getElementById("job-salary").value,
                                User_Id: user.user_Id,
                                Statusz: "nyitott",
                                PosztDatum: await getDate(),
                            }
                            let newjob = await fetchData("Feladat/uj","POST",data)
                            showConfirm('Sikeresen mentetted!', function(igen) {
                            }, "Oké");
                            target.setAttribute("jobid",newjobĐ[0].task_Id)
                        }else{
                            let data = {
                                Statusz: document.getElementById("job-titlename").getAttribute("jobstatus"),
                                Helyszin: document.getElementById("job-location").value,
                                Cim: document.getElementById("job-titlename").value,
                                Hatarido: document.getElementById("job-date").value,
                                Leiras: document.getElementById("job-description").value,
                                Idotartam: document.getElementById("job-time").value,
                                Fizetes: document.getElementById("job-salary").value,
                            }
                            await fetchData(`Feladat/update/${jobid}`,"PATCH",data)
                            showConfirm('Sikeresen mentetted!', function(igen) {
                            }, "Oké");
                        }

                    }
                });
    
            })
            //munka lezárása
            if(!newjob){
                document.getElementById("jobdelete").addEventListener("click",async function(event){
                    const targetevent = event
                    showConfirm('Biztosan lezárod?', async function(igen) {
                        if (igen) {
                            //lezárás
                            jobStatusClose(targetevent)
                        }
                    }); 
                })
            }
        }
    }else{
        showblur(true)
        jobview.innerHTML = `
        <div class="content">
                <div class="job-header-container">
                    <div class="job-header">
                        <h1>${current.cim}</h1>
                        <span class="job-badge" id="job-status">${current.statusz}</span>
                    </div>
                    <div class="job-meta" id="job-categorys">
                        <span>asdsa</span>
                        <span>asdads</span>
                        <span>asdsa</span>
                        <span>asdads</span>
                        <span>asdsa</span>
                        <span>asdads</span>
                        <span>asdsa</span>
                        <span>asdads</span>
                        <span>asdsa</span>
                        <span>asdads</span>
                        
                    </div>
                </div>
                <div class="job-description-container">
                    <h2>Munka leírása:</h2>
                    <p id="job-description">
                        ${current.leiras}
                    </p>
                </div>
            </div>
            <div class="sidebar">
                <div class="job-details-container">
                    <h2>Munka részletei</h2>
                    <p><strong>Meghirdetés dátuma:</strong><span id="job-createdate">${current.posztDatum}</span></p>
                    <p><strong>Helyszín:</strong><span id="job-location">${current.helyszin}</span></p>
                    <p><strong>Fizetés:</strong><span id="job-salary">${current.fizetes} Ft</span></p>
                    <p><strong>Időtartam:</strong><span id="job-time">${current.idotartam}</span></p>
                    <p><strong>Határidő:</strong><span id="job-date">${current.hatarido}</span></p>
                    <p><strong>Hirdető:</strong><span id="jobowner">${current.felhasznalo.vezNev} ${current.felhasznalo.kerNev}</span></p>
                </div>
                <div class="apply-section-container">
                    <h2>Jelentkezés a munkára</h2>
                     ${!current.jelentkezette && current.statusz!="lezárt"
                    ? `<button class="apply-button" id="jobactionbutton" fid="${current.task_Id}">Jelentkezés most →</button>`
                    : `${current.statusz!="lezárt" 
                    ? "<h2>Már jelentkeztél erre a munkára</h2>"
                    : "<h2>Munka lezárva</h2>"}`
                    }
                </div>
            </div>
        `;
        document.getElementById("jobowner").setAttribute("ownerid",current.felhasznalo.user_Id)
        if(!current.jelentkezette && current.statusz!="lezárt"){
            document.getElementById("jobactionbutton").addEventListener("click",async function(target){
                const current = target.currentTarget.getAttribute("fid");
                showConfirm('Biztosan jelentkezel?', async function(igen) {
                    if (igen) {
                        //jelentkezés fetch
                        let data = {Task_Id: current, User_Id: user.user_Id,JelDatum: await getDate()}
                        let jel = await fetchData("jelentkezes/uj","POST",data)
                        //sikeres jelentkezés
                        if(jel!=null){
                            document.getElementById("jobactionbutton").innerText="Jelentkezett"
                            document.getElementById("jobactionbutton").setAttribute("disabled","true")
                        }
                    }
                  });
                
            })
        }

         //hirdeto megtekintese
        document.getElementById("jobowner").addEventListener("click",function(){
            const ownerid = document.getElementById("jobowner").getAttribute("ownerid");

            profileviewopen(ownerid)
        })
    }
    
    var categorys = document.getElementById("job-categorys")
            
    categorys.innerHTML=""
    if(event!=undefined || current.statusz=="lezárt"){
        let cats = current.feladatKategoriak;
        //atalaizas html elemre
        
        let editeable = upanel.classList.contains("show");
        cats.forEach(element => {
            let span = document.createElement("span")
            span.innerText=element.katnev
            if(editeable){
                let x = document.createElement("div")
                x.innerText="X"
                x.setAttribute("katid",element.kat_Id)
                x.setAttribute("jobid",jobdata)
                x.addEventListener("click",async function(event){
                    let katid = event.currentTarget.getAttribute("katid")
                    let jobid = event.currentTarget.getAttribute("jobid")

                    let element = event.currentTarget.parentElement
                    
                    await fetchData(`kategoria/kapcsolattorles/${jobid}/${katid}`,"DELETE");
                    
                    element.remove();
                })
                span.appendChild(x)
            }
            
            categorys.appendChild(span)
        });
    }

    showjobfullview(true)
    scroll(false);
}
/*Profileview*/

function userview(oid){
    showprofileview(true)
}

//application panel

var headerOptions 
// Function to load applications
async function loadApplications() {
    const applicationsContainer = document.getElementById('applications-container');
    const activeHeader = document.querySelector('#application-panel .panel-header .header-option.active');
    const section = activeHeader?.getAttribute('data-section');

    applicationsContainer.innerHTML = ''; // Clear previous applications
    //az ő jelentkezéseit vagy az ő munkájára jelentkezetteket akarja nézni
    const appaction = document.getElementById("application-panel").getAttribute("data-action");
    //received //amit ő kapott az adott munkára
    //sent //amit ő kuldott külön a navbol
    document.getElementById("jobaccept").style.display="none";
    document.getElementById("jobreject").style.display="none";

    document.getElementById("jobdepeding").style.display="none";
    //ez kell adatok feltoltese melyiknel milyen adatokat toltodjon be
    if(appaction == "received"){
        const jobid = document.getElementById("application-panel").getAttribute("jobid")
        

        const appdata = await fetchData(`munkarajelentkezesek/${jobid}`)
        console.log(appdata)
        if(appdata.length!=0){
            if(appdata.some(item => item.statusz=="függőben" && item.latta_e === 0)){
                //ha van olyan amit még nem látott
                document.getElementById("jobdepeding").style.display="block";
            }
            
            appdata.filter(item => item.statusz === section).forEach(app => {
                const appCard = document.createElement('div');
                appCard.classList.add('application-card');

                let summary = document.createElement("div");
                summary.classList.add("application-summary");
                summary.innerHTML = `<span>${app.felhasznalo.teljesNev}</span>`;
                console.log(section)
                if (section === 'függőben' && appaction==='received') {
                    let acceptBtn = document.createElement("button");
                    acceptBtn.classList.add("btn", "btn-accept");
                    acceptBtn.textContent = "Accept";
                    acceptBtn.addEventListener("click", () => {
                        showConfirm('Biztosan elfogadod?', function(igen) {
                            if (igen) {
                                acceptApplication(app.felhasznalo.user_Id,jobid)
                            } 
                        });
                    });
                    summary.appendChild(acceptBtn);
                    
                    let rejectBtn = document.createElement("button");
                    rejectBtn.classList.add("btn", "btn-reject");
                    rejectBtn.textContent = "Reject";
                    rejectBtn.addEventListener("click", () => {
                        showConfirm('Biztosan elutasítod?', function(igen) {
                            if (igen) {
                                rejectApplication(app.felhasznalo.user_Id,jobid)
                            } 
                        });
                    });
                    summary.appendChild(rejectBtn);
                }
                
                let arrow = document.createElement("span");
                arrow.classList.add("application-arrow");
                arrow.textContent = "▼";
                if(section === 'függőben' && appaction==='received'){
                    arrow.setAttribute("seen",app.latta_e)
                }else{
                    arrow.setAttribute("seen","")
                }
                arrow.setAttribute("uid",app.felhasznalo.user_Id)
                arrow.addEventListener("click", toggleDetails);
                summary.appendChild(arrow);

                let details = document.createElement("div");
                details.classList.add("application-details");
                details.innerHTML = `
                    <p><strong>Státus: </strong>${app.statusz}</p>
                    <p><strong>Jelentkezés dátuma: </strong>${app.jelDatum}</p>
                    <p><strong>Telefonszam:</strong>${app.felhasznalo.telefonszam}</p>
                    
                `;
                
                let btn = document.createElement("button")
                btn.innerText="Profil megtekintése"
                btn.classList.add("profilviewbtn")
                btn.setAttribute("uid",app.felhasznalo.user_Id)
                btn.addEventListener("click",function(event){
                    let uid = event.currentTarget.getAttribute("uid")
                    //felhasznalo profil megtekintes
                    profileviewopen(uid)
                })
                details.appendChild(btn)
                appCard.appendChild(summary);
                appCard.appendChild(details);
                
                if (app.latta_e == 0 && section === 'függőben') {
                    let alert = document.createElement("div");
                    alert.classList.add("alert");
                    alert.textContent = "!";
                    appCard.appendChild(alert);
                }
                applicationsContainer.appendChild(appCard);
            });
        }else{

        }
    
    }else{
        //sajat jelentkezesek
        const appdata = await fetchData(`jelentkezesek/${user.user_Id}`)
        const sectiondata = appdata.filter(elem => elem.statusz === section)
        console.log(sectiondata);
        console.log(appdata)

        if(appdata.some(item =>item.section=="elfogadva" && item.latta_e === 0)){
            //ha van olyan amit még nem látott
            document.getElementById("jobaccept").style.display="block";
        }
        if(appdata.some(item =>item.section=="elutasitva" && item.latta_e === 0)){
            //ha van olyan amit még nem látott
            document.getElementById("jobreject").style.display="block";
        }
        sectiondata.forEach(data => {
            const appCard = document.createElement('div');
            appCard.classList.add('application-card');

            let summary = document.createElement("div");
            summary.classList.add("application-summary");
            summary.innerHTML = `<span>${data.feladat.cim}</span>`;
            console.log(section)
            
            let arrow = document.createElement("span");
            arrow.classList.add("application-arrow");
            arrow.textContent = "▼";
            if(section != 'függőben'){
                arrow.setAttribute("seen",data.latta_e)
            }else{
                arrow.setAttribute("seen","")

            }

            arrow.setAttribute("jobid",data.feladat.task_Id)
            arrow.setAttribute("statusz",data.statusz)
            arrow.setAttribute("uid",data.felhasznalo.user_Id)
            arrow.addEventListener("click", toggleDetails);
            summary.appendChild(arrow);
    
            let details = document.createElement("div");
            details.classList.add("application-details");
            details.innerHTML = `
                <p><strong>Helyszín: </strong>${data.feladat.helyszin}</p>
                <p><strong>Jelentkezés dátuma: </strong>${data.jelDatum}</p>
            `;
            let btn = document.createElement("button")
            btn.innerText="Munka megtekintése"
            btn.classList.add("profilviewbtn")
            btn.setAttribute("jobid",data.feladat.task_Id)
            btn.addEventListener("click",function(event){
                //feladat megtekintese
                jobviewopen(event)
            })
            details.appendChild(btn)
            appCard.appendChild(summary);
            appCard.appendChild(details);
            
            if (data.latta_e == 0 && section != 'függőben') {
                let alert = document.createElement("div");
                alert.classList.add("alert");
                alert.textContent = "!";
                appCard.appendChild(alert);
            }
            applicationsContainer.appendChild(appCard);
        })
    }
}

async function toggleDetails(event) {
    let seen = event.currentTarget.getAttribute("seen");
    let arrow = event.currentTarget;
    if(seen == 0){
        let statusz = arrow.getAttribute("statusz")
        let uid = arrow.getAttribute("uid");
        let jobid = document.getElementById("application-panel").getAttribute("jobid");
        if(jobid!=null && statusz =="függőben"){
            await fetchData(`jelentkezes/seen/${uid}/${jobid}`, "PATCH");
        }
        if(jobid == null && statusz !="függőben"){
            jobid = arrow.getAttribute("jobid")
            await fetchData(`jelentkezes/seen/${uid}/${jobid}`, "PATCH");
        }
        arrow.setAttribute("seen", 1);
    }
    let container = arrow.closest('.application-card');
    let details = container.querySelector('.application-details');
    container.classList.toggle('open');
    if (container.classList.contains('open')) {
        details.style.display = 'grid';
        details.style.maxHeight = '0';
        setTimeout(() => {
            details.style.maxHeight = details.scrollHeight + 'px';
            details.style.opacity = '1';
        }, 10);
    } else {
        details.style.maxHeight = '0';
        details.style.opacity = '0';
        setTimeout(() => {
            if (!container.classList.contains('open')) {
                details.style.display = 'none';
            }
        }, 500);
    }
}


// Action functions
async function acceptApplication(id,id2) {
    alert(`Accepting application ${id}`);
    let b = await fetchData(`jelentkezes/action/${id}/${id2}`,"PUT",{Statusz: "elfogadva",Latta_e: 0})
    loadApplications()
}

async function rejectApplication(id,id2) {
    alert(`Rejecting application ${id}`);
    let b = await fetchData(`jelentkezes/action/${id}/${id2}`,"PUT",{Statusz: "elutasítva",Latta_e: 0})
    loadApplications()
}

function applicationsOpen(show){
    if(show){
        document.getElementById("application-panel").classList.add("show")
        document.getElementById("application-panel").style.zIndex = zindex;
        zindex++;
        showblur(true)
    }else{
        document.getElementById("application-panel").classList.remove("show")
        upanelcheck()
    }
}


//profil megtekintése

function profileview(show){
    if(show){
        document.getElementById("profileview").classList.add("show");
        document.getElementById("profileview").style.zIndex = zindex;
        zindex++;
    }else{
        document.getElementById("profileview").classList.remove("show");
    }
    if(scrollactive){
        scroll(true);
        scrollactive = false;
    }
}

async function profileviewopen(uid){
    //ami generalja tolti fel fugveny
    let sudata = await fetchData(`felhasznalo/getuser/${uid}/${user.user_Id}`,"GET")

    document.getElementById("pcard-name").innerHTML=sudata.teljesNev;
    document.getElementById("pcard-name").setAttribute("uid",uid)
    document.getElementById("pcard-bdate").innerHTML=sudata.szulDat;
    document.getElementById("pcard-regdate").innerHTML=sudata.regDatum;

    const stars = document.getElementById("pcard-rating")
    let starCount = sudata.ertekelesek.reduce((acc, curr) => acc + curr.ertekeles, 0) / sudata.ertekelesek.length;
    let starCountRounded = Math.round(starCount * 10) / 10; // Kerekítés egy tizedesjegyre
    stars.innerHTML = `${generateStarsHTML(starCountRounded)} <span>${starCountRounded} (${sudata.ertekelesek.length} értékelés)</span>`;

    document.getElementById("pcard-bio").innerHTML=sudata.bio;
    if(sudata.profilKep!=null){
        document.getElementById("pcard-avatar").src = "https://jannn1.hu/"+sudata.profilKep
    }else{
        document.getElementById("pcard-avatar").src = "https://jannn1.hu/images/hbsz.png"
    }

    document.getElementById("pcard-phone").innerText = sudata.telefonszam

    let jobcont = document.getElementById("pcard-jobs-container")
    jobcont.innerHTML = ""
    sudata.feladatok.forEach(job=> jobcont.appendChild(createjobcard(job)))

    let ratingcontainer = document.getElementById("pcard-ratings-list-container")
    ratingcontainer.innerHTML = ""
    sudata.ertekelesek.forEach(ert=> {
        let ratitem = document.createElement("div")
        ratitem.classList.add("pcard-rating-item")

        let rathead = document.createElement("div")
        rathead.classList.add("pcard-rating-header")

        let uname = document.createElement("span")
        uname.classList.add("pcard-username")
        uname.innerText = ert.teljesNev;
        rathead.appendChild(uname)

        let ustar = document.createElement("span")
        ustar.classList.add("pcard-stars")
        ustar.innerHTML = generateStarsHTML(ert.ertekeles)
        ustar.setAttribute("data-rating",ert.ertekeles)
        rathead.appendChild(ustar)

        ratitem.appendChild(rathead)

        let ratdesc = document.createElement("p")
        ratdesc.classList.add("pcard-description")
        ratdesc.innerText = ert.comment;

        ratitem.appendChild(ratdesc)

        ratingcontainer.append(ratitem)
        
    })

    profileview(true)
}

// Csillagok generálása
function generateStarsHTML(score, maxStars = 5) {
    let starsHTML = '';
    for (let i = 1; i <= maxStars; i++) {
        starsHTML += `<i class="fas fa-star${i <= score ? '' : ' ' + 'pcard-star-empty'}"></i>`;
    }
    return starsHTML;
}

var tabs
var tabContents

var newRatingForm
var starInputContainer
var ratingStarsInput
var hiddenScoreInput

function initProfileCardInteractions() {
    const newRatingForm = document.getElementById('pcard-new-rating-form');
    const ratingsListContainer = document.getElementById('pcard-ratings-list-container');
    const starInputContainer = document.querySelector('.pcard-star-input-container');
    const ratingStarsInput = starInputContainer ? starInputContainer.querySelectorAll('i') : [];
    const hiddenScoreInput = document.getElementById('pcard-rating-score-hidden');

    document.querySelectorAll('.pcard-ratings-list .pcard-stars[data-rating]').forEach(starContainer => {
        const rating = parseInt(starContainer.dataset.rating, 10);
        if (!isNaN(rating)) {
            starContainer.innerHTML = generateStarsHTML(rating);
        }
    });
}

function showConfirm(text, onConfirm, okeText) {
    const overlay = document.getElementById('overlay');
    const panel = document.getElementById('confirmPanel');
    const textElem = document.getElementById('confirmText');
    const yesBtn = document.getElementById('yesBtn');
    const noBtn = document.getElementById('noBtn');
  
    textElem.textContent = text;
    overlay.style.display = 'block';
    overlay.zindex = zindex;
    zindex++;
    panel.style.display = 'block';
    panel.zIndex = zindex;
    zindex++;

    if(okeText != null){
        yesBtn.innerText = okeText
        noBtn.style.display = "none"
    }else{
        yesBtn.innerText = "Igen"
        noBtn.style.display = "relative"
    }


    setTimeout(() => {
      overlay.style.opacity = '1';
      panel.style.opacity = '1';
      panel.style.transform = 'translate(-50%, -50%) scale(1)';
    }, 10);
  
    function cleanUp() {
      overlay.style.opacity = '0';
      panel.style.opacity = '0';
      panel.style.transform = 'translate(-50%, -50%) scale(0.8)';
      
      setTimeout(() => {
        overlay.style.display = 'none';
        panel.style.display = 'none';
        yesBtn.removeEventListener('click', onYes);
        noBtn.removeEventListener('click', onNo);
      }, 300);
    }
  
    function onYes() {
      cleanUp();
      onConfirm(true);
    }
  
    function onNo() {
      cleanUp();
      onConfirm(false);
    }
  
    yesBtn.addEventListener('click', onYes);
    noBtn.addEventListener('click', onNo);
}

