function togglePanel() {
    const container = document.getElementById('panel-container');
    container.classList.toggle('rotated');
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

let apiurl = 'https://localhost:44300/api/';
// Bejelentkezés kezelése
async function login() {
    //Hibak resetelése
    document.querySelector('#loginerror').style.display = 'none';
    document.querySelector('#loginallerror').style.display = 'none';
    
    //Adatok bekerese
    const email = document.querySelector('#logmail').value;
    const password = document.querySelector('#logpw').value;
    const rememberMe = document.querySelector('#logremember').checked;

    //Hibak elemzése
    if (!email || !password) {
        document.querySelector('#loginallerror').style.display = 'block';
        return; 
    }
    //Adatok küldése a backendnek
    var resp = await fetch(`${apiurl}felhasznalo/login/${email}/${password}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
    })
    
    //Válaszának elemzése
    if(resp.ok){
        //Felhasznalo elmentese hogy bejelentkezve maradjon
        if(rememberMe){
            localStorage.setItem('jobsearchuser',JSON.stringify({email,password}))
        }
        //Felhasznalo elmentése a munkamenet idejére
        sessionStorage.setItem('jobsearchuser',JSON.stringify({email,password}))

        window.location.href = './index.html';
    }else{
        document.querySelector('#loginerror').style.display = 'block';
    }
}

// Regisztráció kezelése
async function register() {
    //Hibak resetelése
    document.querySelector('#regallerror').style.display = 'none';
    document.querySelector('#regpwerror').style.display = 'none';
    document.querySelector('#regerror').style.display = 'none';

    //Adatok bekerese
    const email = document.getElementById('regemail').value;
    const password = document.getElementById('regpw').value;
    const confirmPassword = document.getElementById('regpwconfirm').value;
    const rememberMe = document.getElementById('regremember').checked;

    //Hibak elemzése
    if (!email || !password || !confirmPassword) {
        document.querySelector('#regallerror').style.display = 'block';
        return;
    }
    if (password !== confirmPassword) {
        document.querySelector('#regpwerror').style.display = 'block';
        return;
    }

    //Adatok küldése a backendnek
    var resp = await fetch(apiurl+'register', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({Email: email, Jelszo: password, RegDatum: getDate()}),
    })
    console.log(resp)
    //Válaszának elemzése
    if(resp.ok){
        alert("Sikeres regisztáció!")
        //Felhasznalo elmentese hogy bejelentkezve maradjon
        if(rememberMe){
            localStorage.setItem('jobsearchuser',JSON.stringify({email,password}))
        }
        //Felhasznalo elmentése a munkamenet idejére
        sessionStorage.setItem('jobsearchuser',JSON.stringify({email,password}))

        //window.location.href = './index.html';
    }else{
        document.querySelector('#regerror').style.display = 'block';
    }
}

