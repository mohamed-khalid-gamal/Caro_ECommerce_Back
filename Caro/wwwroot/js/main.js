let navs = document.querySelectorAll("#header .navbar li");
let bars = document.querySelector("#header .bar");
let close = document.querySelector("#header .navbar .close");
let nav = document.querySelector("#header .navbar");
let products = document.querySelectorAll(".products .pro");
let proimg = document.querySelector("#pro-details > div.images > img");
let imgs = document.querySelectorAll("#pro-details .images .alters img");
let loader = document.querySelector(".p1");
let cont = document.querySelector(".container-web");
if (bars || close) {
    bars.addEventListener("click", function () {
        nav.style.width = "40%";
    });
    close.addEventListener("click", function () {
        nav.style.width = "0%";
    });
}
for (i = 0; i < 6; i++) {
    if (
        location.pathname.includes("/Shop/Index")
    ) {
        navs.forEach((x) => x.classList.remove("active"));
        navs[0].classList.add("active");
        break;
    } else if (location.pathname.includes("/Shop/Products")) {
        navs.forEach((x) => x.classList.remove("active"));
        navs[1].classList.add("active");
        break;
    }else if (location.pathname.includes("/Shop/Blogs")) {
        navs.forEach((x) => x.classList.remove("active"));
        navs[2].classList.add("active");
        break;
    }else if (location.pathname.includes("/Shop/About")) {
        navs.forEach((x) => x.classList.remove("active"));
        navs[3].classList.add("active");
        break;
    }else if (location.pathname.includes("/Shop/Contact")) {
        navs.forEach((x) => x.classList.remove("active"));
        navs[4].classList.add("active");
        break;
    }else if (location.pathname.includes("/Cart")) {
        navs.forEach((x) => x.classList.remove("active"));
        navs[navs.length-1].classList.add("active");
        break;
    }
}
if (imgs) {
    imgs.forEach((e) => {
        e.addEventListener("click", function () {
            var x = proimg.src;
            proimg.src = e.src;
            e.src = x;
        });
    });
}
window.onload = function () {
    loader.style.animation = "load 2s 1s ease-in forwards";
    cont.style.display = "block";
};
