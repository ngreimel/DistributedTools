let userId = null
let isAdmin = false
let currentItem = null

const voteMap = [
    {vote: 1, name: 'tell', display: 'Tell'},
    {vote: 2, name: 'sell', display: 'Sell'},
    {vote: 3, name: 'consult', display: 'Consult'},
    {vote: 4, name: 'agree', display: 'Agree'},
    {vote: 5, name: 'advise', display: 'Advise'},
    {vote: 6, name: 'inquire', display: 'Inquire'},
    {vote: 7, name: 'delegate', display: 'Delegate'}
]

const init = async () => {
    console.log('init')

    userId = localStorage.getItem('userId')
    console.log(userId)
    
    setInterval(updateState, 1000)
    //updateState()
}

const updateState = async () => {
    const data = await get('/api/state')
    //console.log(data)

    currentItem = data.items.find(x => x.itemId === data.currentItemId)
    const user = data.users.find(x => x.userId === userId)
    if (user) {
        isAdmin = user.type === 1
        //document.getElementById('register').classList.add('hidden')
        if (currentItem) {
            document.getElementById('cards').classList.remove('hidden')
        }
    }
    
    updateUsers(data)
    updateCurrentItem(data)
    updateItemList(data)
}

const updateUsers = (data) => {
    const rows = data.users.map(user => {
        const currentUserClass = user.userId === userId ? ' current-user' : ''
        const userVoted = currentItem && currentItem.votes.some(x => x.userId === userId)
        const votedClass = userVoted  ? ' voted' : ''
        return `<div class="user${currentUserClass}${votedClass}">${user.name}</div>`
    })
    document.getElementById('users').innerHTML = rows.join('')
}

const updateCurrentItem = (data) => {
    if (currentItem) {
        document.getElementById('current-item').innerHTML = currentItem.description
        Array.from(document.getElementsByClassName('card')).forEach(x => {
            x.classList.remove('selected')
        })
        const userVote = currentItem.votes.find(x => x.userId === userId)
        if (userVote) {
            var voteData = voteMap.find(x => x.vote === userVote.vote)
            if (voteData) {
                document.getElementsByClassName(voteData.name)[0].classList.add('selected')    
            }
        }
    } else {
        const noItemSelected = `<span class="aside">No decision has been selected for discussion yet...</span>`
        document.getElementById('current-item').innerHTML = noItemSelected
    }
} 

const updateItemList = (data) => {
    const rows = data.items.map(item => {
        const clickableClass = isAdmin ? ' clickable' : ''
        const onClick = isAdmin ? ` onclick="selectItem('${item.itemId}')"` : ''
        return `<div class="item${clickableClass}"${onClick}>${item.description}</div>`
    })
    document.getElementById('itemList').innerHTML = rows.join('')
}

const register = async (event) => {
    const name = document.getElementById('username').value
    if (name) {
        var url = event.altKey ? 'api/register-admin' : '/api/register'
        const data = await post(url, {name})
        userId = data.userId
        localStorage.setItem('userId', userId)
    }
}

const vote = async (value) => {
    await post('/api/vote', {
        itemId: currentItem.itemId,
        userId,
        vote: value
    })
}

const addItem = async () => {
    const description = document.getElementById('newItemDescription').value
    await post('/api/add-item', {description})
}

const selectItem = async (itemId) => {
    await post('/api/set-current-item', {itemId, userId})
}

const get = async (url) => {
    return await makeRequest(url, {
        method: 'GET'
    })
}

const post = async (url, data) => {
    return await makeRequest(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    })
}

const makeRequest = async (url, options) => {
    var response = await fetch(url, {
        ...options,
        cache: 'no-cache',
        referrerPolicy: 'no-referrer'
    })
    return await response.json()
}