
const __closeBtnCss = ``;

function showPopupWindow(gatewayCard) {

    this.createInformationRow = (title, value) => {
        var row = document.createElement('tr');
        var th = document.createElement('th');
        var co = document.createElement('td');
        var td = document.createElement('td');

        th.style.padding = td.style.padding = "1px";
        th.textContent = title
        co.textContent = ":"
        td.textContent = value

        row.appendChild(th);
        row.appendChild(co);
        row.appendChild(td);
        return row;
    }

    const container = document.createElement("div");
    container.style.width = "80vw";
    container.style.height = "80vh";
    container.style.position = "fixed";
    container.style.left = "10vw";
    container.style.top = "10vh";
    container.style.padding = "12px";
    container.style.backgroundColor = "#F0F0F0";
    container.style.boxShadow ="5px 5px 5px #E0E0E0";

    const topbar = document.createElement("div");
    topbar.style.width = "100%";
    topbar.style.height = "50px";

    const btnClose = document.createElement("a");
    btnClose.classList.add("popup-window-close");
    btnClose.addEventListener("click", () => container.remove());
    topbar.appendChild(btnClose);

    const icon = document.createElement('div');
    icon.style.maxWidth = "100px";
    icon.style.maxHeight = "auto";
    icon.style.padding = "12px";
    icon.style.display = "inline-block";
    icon.style.verticalAlign = "center";
    const svgIcon = document.adoptNode(
        new DOMParser().parseFromString('<svg class="gateway-icon" enable-background="new 0 0 512.006 512.006" viewBox="0 0 512.006 512.006" width="100" xmlns="http://www.w3.org/2000/svg"><g><path d="m217.112 217.133c-5.858 5.857-5.858 15.355 0 21.213 5.857 5.857 15.355 5.857 21.213 0 9.771-9.77 25.584-9.771 35.355 0 5.858 5.859 15.355 5.858 21.213 0 5.858-5.857 5.858-15.355 0-21.213-21.444-21.445-56.336-21.445-77.781 0z" /><path d="m340.148 193.091c5.858-5.857 5.858-15.355 0-21.213-46.397-46.398-121.894-46.398-168.291 0-5.858 5.857-5.858 15.355 0 21.213 5.857 5.857 15.355 5.857 21.213 0 34.701-34.701 91.164-34.701 125.865 0 5.859 5.858 15.356 5.857 21.213 0z" /><path d="m147.815 147.836c59.793-59.792 156.575-59.799 216.375 0 5.856 5.857 15.354 5.86 21.213 0 5.858-5.857 5.858-15.355 0-21.213-71.515-71.516-187.275-71.526-258.801 0-5.858 5.857-5.858 15.355 0 21.213 5.858 5.857 15.356 5.857 21.213 0z" /><path d="m497.003 281.023h-87.78l100.961-185.84c3.955-7.279 1.259-16.386-6.02-20.341s-16.387-1.26-20.341 6.021l-108.741 200.16h-238.158l-108.74-200.16c-3.955-7.28-13.061-9.972-20.341-6.021-7.279 3.955-9.975 13.062-6.02 20.341l100.961 185.84h-87.78c-8.284 0-15 6.716-15 15v128c0 8.284 6.716 15 15 15h482c8.284 0 15-6.716 15-15v-128c-.001-8.284-6.717-15-15.001-15zm-113 94h-256c-8.284 0-15-6.716-15-15s6.716-15 15-15h256c8.284 0 15 6.716 15 15s-6.716 15-15 15z" /></g></svg>'
            , "image/svg+xml").documentElement);
    icon.appendChild(svgIcon);

    const table = document.createElement('table');
    const tbody = document.createElement('tbody');
    table.appendChild(tbody);
    table.style.display = "inline-block";
    table.style.padding = "8px";
    table.style.marginLeft = "20px";
    tbody.style.width = "100%";

    const map = document.createElement('div');
    map.style.width = "100%";
    map.style.height = `calc(100% - 190px)`;
    map.style.border = "1px solid #C0C0C0";

    gatewayCard.information.forEach(element => {
        tbody.appendChild(this.createInformationRow(element.key, element.value))
    });
    svgIcon.setAttribute("fill", gatewayCard.iconColor);

    gatewayCard.addEventListener("refresh", (e) => {
        while (tbody.firstChild)
            tbody.removeChild(tbody.firstChild);

        gatewayCard.information.forEach(element => {
            tbody.appendChild(this.createInformationRow(element.key, element.value))
        });

        svgIcon.setAttribute("fill", gatewayCard.iconColor);
    });

    container.appendChild(topbar);
    container.appendChild(icon);
    container.appendChild(table);
    container.appendChild(map);
    document.body.appendChild(container);

    const gps = [gatewayCard.lat, gatewayCard.lng];
    const mapInstance = L.map(map).setView(gps, 16.5);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    }).addTo(mapInstance);

    var marker = L.marker(gps);
    marker.addTo(mapInstance);
}