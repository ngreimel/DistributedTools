let roomCode
let userId

const adminUserType = 1

const voteTypes = {
    'thumb': 0,
    'fistToFive': 1
}

const connection = new signalR.HubConnectionBuilder().withUrl("/voting-hub").build()
connection.on("StateUpdate", (data) => {
    updateState(data)
})

const init = (newRoomCode, newUserId) => {
    roomCode = newRoomCode
    userId = newUserId
    connection.start().then(() => {
        connection.invoke("ConnectToRoom", roomCode)
    })
}

const updateState = (data) => {
    data.users.sort((a, b) => a.name.localeCompare(b))
    updateUsers(data)
    updatePrompt(data)
    updateVotingResults(data)
    updateControls(data)
}

const updateUsers = (data) => {
    const rows = data.users.map(user => {
        const currentUserClass = user.userId === userId ? ' current-user' : ''
        const userVoted = getVotes(data).some(x => x.userId === user.userId)
        const votedClass = userVoted  ? ' voted' : ''
        return `<div class="user${currentUserClass}${votedClass}">${user.name}</div>`
    })
    document.getElementById('users').innerHTML = rows.join('')
}

const getVotes = (data) => {
    if (data.voteType === voteTypes.fistToFive) {
        return data.fistToFiveVotes
    }
    return data.thumbVotes
}

const updatePrompt = (data) => {
    document.getElementById('prompt').innerHTML = data.prompt;
}

const updateVotingResults = (data) => {
    let voteHtml = ''
    if (data.votesVisible)
    {
        voteHtml = data.voteType === voteTypes.thumb ?
            buildThumbVotingResults(data) :
            buildFistToFiveVotingResults(data)
    }
    document.getElementById('votingResults').innerHTML = voteHtml
}

const buildThumbVotingResults = (data) => {
    const voteCounts = data.thumbVotes.reduce((aggregate, x) => {
        aggregate[x.vote].count++;
        aggregate[x.vote].users.push(getUserName(data, x.userId))
        return aggregate
    }, [
        { count: 0, users: [], emoji: '👍' },
        { count: 0, users: [], emoji: '👍', rotated: true },
        { count: 0, users: [], emoji: '👎' }
    ])

    return voteCounts.map(x => {
        if (x.count < 1) {
            return ''
        }
        const divClass = x.rotated ? ' class="rotated"' : ''
        const thumbs = new Array(x.count).fill(`<div${divClass}>${x.emoji}</div>`).join('')
        return `<div class="thumb-row">${thumbs}</div><div>${x.users.join(', ')}</div>`
    }).join('')
}

const buildFistToFiveVotingResults = (data) => {
    const voteCounts = data.fistToFiveVotes.reduce((aggregate, x) => {
        aggregate[x.vote].count++;
        aggregate[x.vote].users.push(getUserName(data, x.userId))
        return aggregate
    }, [
        { count: 0, users: [], emoji: '✊' },
        { count: 0, users: [], emoji: '☝' },
        { count: 0, users: [], emoji: '✌' },
        { count: 0, users: [], emoji: '👌' },
        { count: 0, users: [], emoji: '🤚' },
        { count: 0, users: [], emoji: '🖐' }
    ])

    return voteCounts.map(x => {
        if (x.count < 1) {
            return ''
        }
        const votes = new Array(x.count).fill(`${x.emoji}`).join('')
        return `<div>${votes}</div><div>${x.users.join(', ')}</div>`
    }).join('')
}

const getUserName = (data, userId) => {
    const user = data.users.find(x => x.userId === userId)
    return user ? user.name : ''
}

const updateControls = (data) => {
    const user = data.users.find(x => x.userId == userId)
    const isFistToFive = data.voteType === voteTypes.fistToFive

    setVisibility('joinControls', userId && !user)
    setVisibility('votingControls', user)
    setVisibility('thumbVotingControls', !isFistToFive)
    setVisibility('fistToFiveVotingControls', isFistToFive)
    setVisibility('adminControls', isCurrentUserAdmin(data))
}

const setVisibility = (elementId, shouldShow) => {
    if (shouldShow) {
        document.getElementById(elementId).classList.remove('hidden')
    } else {
        document.getElementById(elementId).classList.add('hidden')
    }
}

const isCurrentUserAdmin = (data) => {
    const user = data.users.find(x => x.userId === userId)
    if (user) {
        return user.type === adminUserType
    }
    return false
}

const join = () => {
    connection.invoke("Join", roomCode, userId)
}

const thumbVoteMap = {
    'Up': 0,
    'Sideways': 1,
    'Down': 2
}

const thumbVote = (vote) => {
    connection.invoke("ThumbVote", roomCode, userId, thumbVoteMap[vote])
}

const fistVote = (vote) => {
    connection.invoke("FistToFiveVote", roomCode, userId, vote)
}

const showVotes = () => {
    connection.invoke("MakeVotesVisible", roomCode, userId)
}

const newThumbVote = () => {
    const prompt = document.getElementById('newPrompt').value
    connection.invoke("NewVote", roomCode, userId, voteTypes.thumb, prompt)
}

const newFistVote = () => {
    const prompt = document.getElementById('newPrompt').value
    connection.invoke("NewVote", roomCode, userId, voteTypes.fistToFive, prompt)
}
