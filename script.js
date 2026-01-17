let orders = [];

function orderItem(name, price) {
    let existing = orders.find(item => item.name === name);

    if (existing) {
        existing.qty++;
        existing.total = existing.qty * existing.price;
    } else {
        orders.push({
            name: name,
            price: price,
            qty: 1,
            total: price
        });
    }

    updateTable();
}

function updateTable() {
    let tbody = document.querySelector("#orderTable tbody");
    tbody.innerHTML = "";

    let grandTotal = 0;

    orders.forEach((item, index) => {
        grandTotal += item.total;

        let row = `
            <tr>
                <td>${item.name}</td>
                <td>${item.price} Ks</td>
                <td>
                    <button class="qty-btn" onclick="changeQty(${index}, -1)">-</button>
                    ${item.qty}
                    <button class="qty-btn" onclick="changeQty(${index}, 1)">+</button>
                </td>
                <td>${item.total} Ks</td>
                <td><button onclick="removeItem(${index})">X</button></td>
            </tr>
        `;

        tbody.innerHTML += row;
    });

    document.getElementById("totalAmount").textContent = grandTotal;
}

function changeQty(index, amount) {
    orders[index].qty += amount;

    if (orders[index].qty <= 0) {
        orders.splice(index, 1);
    } else {
        orders[index].total = orders[index].qty * orders[index].price;
    }

    updateTable();
}

function removeItem(index) {
    orders.splice(index, 1);
    updateTable();
}