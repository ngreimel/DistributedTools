let userId = null
let isAdmin = false
let currentItem = null
let roomCode = null
let baseUrl = null

const voteMap = [
    {vote: 1, name: 'tell', display: 'Tell'},
    {vote: 2, name: 'sell', display: 'Sell'},
    {vote: 3, name: 'consult', display: 'Consult'},
    {vote: 4, name: 'agree', display: 'Agree'},
    {vote: 5, name: 'advise', display: 'Advise'},
    {vote: 6, name: 'inquire', display: 'Inquire'},
    {vote: 7, name: 'delegate', display: 'Delegate'}
]

const connection = new signalR.HubConnectionBuilder().withUrl("/decision-delegation-hub").build()
connection.on("StateUpdate", (data) => {
    updateState(data)
})

const init = (newRoomCode, newUserId) => {
    roomCode = newRoomCode
    baseUrl = `/decision-delegation/${roomCode}`
    userId = newUserId
    connection.start().then(() => {
        connection.invoke("ConnectToRoom", roomCode)
    })
}

const updateState = (data) => {
    currentItem = data.items.find(x => x.itemId === data.currentItemId)
    const user = data.users.find(x => x.userId === userId)
    if (user) {
        isAdmin = user.type === 1
        if (isAdmin && currentItem && !currentItem.isVisible) {
            document.getElementById('displayVotes').classList.remove('hidden')
        } else {
            document.getElementById('displayVotes').classList.add('hidden')
        }
        document.getElementById('join').classList.add('hidden')
        if (currentItem) {
            document.getElementById('cards').classList.remove('hidden')
        }
    } else {
        if (userId) {
            document.getElementById('join').classList.remove('hidden')
        } else {
            document.getElementById('join').classList.add('hidden')
        }
    }

    updateUsers(data)
    updateCurrentItem(data)
    updateItemList(data)
    updateDiscussedItemList(data)
}

const updateUsers = (data) => {
    const rows = data.users.map(user => {
        const currentUserClass = user.userId === userId ? ' current-user' : ''
        const userVoted = currentItem && currentItem.votes.some(x => x.userId === user.userId)
        const votedClass = userVoted  ? ' voted' : ''
        return `<div class="user${currentUserClass}${votedClass}">${user.name}</div>`
    })
    document.getElementById('users').innerHTML = rows.join('')
}

const updateCurrentItem = (data) => {
    if (currentItem) {
        const currentItemHtml = currentItem.description + buildVotes(data)
        document.getElementById('current-item').innerHTML = currentItemHtml

        Array.from(document.getElementsByClassName('card')).forEach(x => {
            x.classList.remove('selected')
        })
        const userVote = currentItem.votes.find(x => x.userId === userId)
        if (userVote) {
            var voteData = voteMap.find(x => x.vote === userVote.vote)
            if (voteData) {
                document.getElementsByClassName(`card ${voteData.name}`)[0].classList.add('selected')
            }
        }
    } else {
        const noItemSelected = `<span class="aside">No decision has been selected for discussion yet...</span>`
        document.getElementById('current-item').innerHTML = noItemSelected
    }
}

const buildVotes = (data) => {
    if (currentItem.isVisible) {
        const votes = voteMap.map(x => ({
            ...x,
            count: currentItem.votes.filter(v => v.vote == x.vote).length
        }))
        const totalVotes = currentItem.votes.length
        const voteRows = votes.map(x => {
            const percent = Math.round(x.count / totalVotes * 100) / 100;
            const width = Math.round(250 * percent)
            return `<div>` +
                `<span class="vote-display">${x.display}</span>` +
                `<span class="vote-count">${x.count}</span>` +
                `<span class="vote-bar ${x.name}" style="width: ${width}px;">&nbsp;</span>` +
                `</div>`
        }).join('')
        return `<div class="vote-results">${voteRows}</div>`
    }
    return ''
}

const updateItemList = (data) => {
    const rows = data.items.filter(x => !x.isVisible).map(item => {
        const clickableClass = isAdmin ? ' clickable' : ''
        const onClick = isAdmin ? ` onclick="selectItem('${item.itemId}')"` : ''
        return `<div class="item${clickableClass}"${onClick}>${item.description}</div>`
    })
    document.getElementById('itemList').innerHTML = rows.join('')
}

const updateDiscussedItemList = (data) => {
    const rows = data.items.filter(x => x.isVisible).map(item => {
        const votes = voteMap.map(v => ({...v, count: item.votes.filter(y => y.vote === v.vote).length}))
        votes.sort((a,b) => a.count > b.count ? -1 : 1)
        const maxCount = votes[0].count
        const result = votes.filter(v => v.count == maxCount).map(v => v.display)
        const clickableClass = isAdmin ? ' clickable' : ''
        const onClick = isAdmin ? ` onclick="selectItem('${item.itemId}')"` : ''
        return `<div class="item${clickableClass}"${onClick}>${item.description} (${result.join(', ')})</div>`
    })
    document.getElementById('discussedItemList').innerHTML = rows.join('')
}

const join = () => {
    connection.invoke("Join", roomCode, userId)
}

const vote = (value) => {
    connection.invoke("Vote", roomCode, currentItem.itemId, userId, value)
}

const displayVotes = () => {
    connection.invoke("MakeVisible", roomCode, currentItem.itemId, userId)
}

const addItem = () => {
    const element = document.getElementById('newItemDescription')
    if (element.value) {
        const descriptions = element.value.split('|')
        for (let i = 0; i < descriptions.length; i++)
        {
            const description = descriptions[i].trim()
            if (description.length > 0) {
                connection.invoke("AddItem", roomCode, description)
            }
        }
        element.value = ''
    }
}

const selectItem = (itemId) => {
    connection.invoke("SetCurrentItem", roomCode, itemId, userId)
}
