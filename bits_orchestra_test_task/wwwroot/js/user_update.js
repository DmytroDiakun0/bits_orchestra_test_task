document.addEventListener('DOMContentLoaded', function () {
    const table = document.getElementById('user-table');

    table.addEventListener('click', function (e) {
        if (e.target.classList.contains('editable')) {
            let td = e.target;
            let currentValue = td.textContent;
            let input = document.createElement('input');
            input.type = 'text';
            input.value = currentValue;
            input.className = 'form-control';
            td.textContent = '';
            td.appendChild(input);

            input.focus();

            input.addEventListener('blur', function () {
                let newValue = input.value;
                let userId = td.parentElement.getAttribute('data-id');
                let field = td.getAttribute('data-field');

                td.textContent = newValue;

                fetch('/User/Update', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'X-CSRF-TOKEN': '@Html.AntiForgeryToken()'
                    },
                    body: JSON.stringify({
                        id: userId,
                        field: field,
                        value: newValue
                    })
                }).then(response => response.json())
                    .then(data => {
                        if (!data.success) {
                            alert('Error updating value');
                            td.textContent = currentValue;
                        }
                    });
            });

            input.addEventListener('keydown', function (event) {
                if (event.key === 'Enter') {
                    input.blur();
                }
            });
        }
    });
});