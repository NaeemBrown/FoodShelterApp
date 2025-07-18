function updateInventoryItem(itemId, updates) {
    return fetch(`/api/inventory/${itemId}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(updates)
    })
        .then(response => response.json());
}

function deleteInventoryItem(itemId) {
    return fetch(`/api/inventory/${itemId}`, {
        method: 'DELETE'
    })
        .then(response => response.json());
}

function addInventoryItem(item) {
    return fetch('/api/inventory', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(item)
    })
        .then(response => response.json());
}