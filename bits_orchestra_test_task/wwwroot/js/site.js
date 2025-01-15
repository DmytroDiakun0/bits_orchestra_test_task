function sortTable(n) {
    let switchcount = 0;

    let table = document.getElementById("user-table");
    let switching = true;
    let dir = "asc";

    while (switching) {
        switching = false;
        let rows = table.rows;

        let shouldSwitch;
        let i, x, y;
        for (i = 1; i < (rows.length - 1); i++) {
            shouldSwitch = false;
            x = rows[i].getElementsByTagName("TD")[n];
            y = rows[i + 1].getElementsByTagName("TD")[n];

            if (dir === "asc") {
                if (x.innerHTML.toLowerCase() > y.innerHTML.toLowerCase()) {
                    shouldSwitch = true;
                    break;
                }
            } else if (dir === "desc") {
                if (x.innerHTML.toLowerCase() < y.innerHTML.toLowerCase()) {
                    shouldSwitch = true;
                    break;
                }
            }
        }

        if (shouldSwitch) {
            rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
            switching = true;
            switchcount++;
        } else if (switchcount === 0 && dir === "asc") {
            dir = "desc";
            switching = true;
        }
    }

    let headers = table.getElementsByTagName("th");
    let i;
    for (i = 0; i < headers.length; i++) {
        headers[i].classList.remove("fw-bold", "text-danger", "sorted-asc", "sorted-desc");
    }
    
    let header = headers[n];
    header.classList.add("fw-bold");
    header.classList.add("text-danger");
    header.classList.add(dir === "asc" ? "sorted-asc" : "sorted-desc");
}