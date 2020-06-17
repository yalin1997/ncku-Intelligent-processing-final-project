class GatewayCard extends HTMLElement {
    constructor() {
        super();

        const shadow = this.attachShadow({ mode: 'open' });
        const style = document.createElement('style');
        style.textContent = `
                            .gateway-icon-wrapper {
                                width: 100%;
                                display: flex;
                                justify-content: center;
                                align-items: center;
                            }
                            .gateway-icon {
                                width: 60%;
                                max-width: 80px;
                            }
                            .gateway-icon-danger {
                                fill: red;
                            }
                            .gateway-title {
                                width: 100%;
                            }
                            .gateway-card-information {
                                width: 100%;
                            }
                        `;

        var title = document.createElement('div');
        title.classList.add('gateway-title');
        title.innerHTML = 'Gateway';

        var icon = document.createElement('div');
        icon.classList.add('gateway-icon-wrapper');
        this._svgIcon = document.adoptNode(
            new DOMParser().parseFromString('<svg class="gateway-icon" enable-background="new 0 0 512.006 512.006" viewBox="0 0 512.006 512.006" width="512" xmlns="http://www.w3.org/2000/svg"><g><path d="m217.112 217.133c-5.858 5.857-5.858 15.355 0 21.213 5.857 5.857 15.355 5.857 21.213 0 9.771-9.77 25.584-9.771 35.355 0 5.858 5.859 15.355 5.858 21.213 0 5.858-5.857 5.858-15.355 0-21.213-21.444-21.445-56.336-21.445-77.781 0z" /><path d="m340.148 193.091c5.858-5.857 5.858-15.355 0-21.213-46.397-46.398-121.894-46.398-168.291 0-5.858 5.857-5.858 15.355 0 21.213 5.857 5.857 15.355 5.857 21.213 0 34.701-34.701 91.164-34.701 125.865 0 5.859 5.858 15.356 5.857 21.213 0z" /><path d="m147.815 147.836c59.793-59.792 156.575-59.799 216.375 0 5.856 5.857 15.354 5.86 21.213 0 5.858-5.857 5.858-15.355 0-21.213-71.515-71.516-187.275-71.526-258.801 0-5.858 5.857-5.858 15.355 0 21.213 5.858 5.857 15.356 5.857 21.213 0z" /><path d="m497.003 281.023h-87.78l100.961-185.84c3.955-7.279 1.259-16.386-6.02-20.341s-16.387-1.26-20.341 6.021l-108.741 200.16h-238.158l-108.74-200.16c-3.955-7.28-13.061-9.972-20.341-6.021-7.279 3.955-9.975 13.062-6.02 20.341l100.961 185.84h-87.78c-8.284 0-15 6.716-15 15v128c0 8.284 6.716 15 15 15h482c8.284 0 15-6.716 15-15v-128c-.001-8.284-6.717-15-15.001-15zm-113 94h-256c-8.284 0-15-6.716-15-15s6.716-15 15-15h256c8.284 0 15 6.716 15 15s-6.716 15-15 15z" /></g></svg>'
                , "image/svg+xml").documentElement);
        icon.appendChild(this._svgIcon);

        var table = document.createElement('table');
        var tbody = document.createElement('tbody');
        table.appendChild(tbody);
        table.classList.add('gateway-card-information');
        this._tbody = tbody;

        shadow.appendChild(style);
        shadow.appendChild(title);
        shadow.appendChild(icon);
        shadow.appendChild(table);
    }

    connectedCallback() {
        this.classList.add('gateway-card');
        this.classList.add('ms-depth-4');
    }

    createInformationRow(title, value) {
        var row = document.createElement('tr');
        var th = document.createElement('th');
        var td = document.createElement('td');

        th.textContent = title
        td.textContent = value

        row.appendChild(th);
        row.appendChild(td);
        return row;
    }

    set iconColor(value) {
        this._svgIcon.setAttribute("fill", value);
        this._iconColor = value;
    }

    get iconColor() {
        return this._iconColor;
    }

    get information() {
        return this._information;
    }

    setInformation(values) {
        while (this._tbody.firstChild)
            this._tbody.removeChild(this._tbody.firstChild);

        values.forEach(element => {
            this._tbody.appendChild(this.createInformationRow(element.key, element.value))
        });

        this._information = values;

        const event = new CustomEvent("refresh", {
            information: values,
            iconColor: this._iconColor
        });

        this.dispatchEvent(event);
    }
}

customElements.define('gateway-card', GatewayCard);