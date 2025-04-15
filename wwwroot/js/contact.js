// 儲存當前使用者名稱和角色清單
let currentUserName = '遊客';
let currentUserRoles = [];

// 頁面載入時初始化留言列表和事件綁定
document.addEventListener('DOMContentLoaded', function () {
    // 獲取當前使用者資訊
    fetch('/KoaLaDessertWeb/Contact/GetCurrentUser')
        .then(response => response.json())
        .then(data => {
            console.log('GetCurrentUser 回應:', data); // 調試用
            if (data.state === 'Normal' && data.message === 'Success') {
                currentUserName = data.results; // 儲存 UserName
                currentUserRoles = data.userRoleList || []; // 儲存角色清單
                console.log('currentUserName:', currentUserName, 'currentUserRoles:', currentUserRoles); // 調試用
            } else {
                console.error('獲取使用者資訊失敗:', data.message);
            }
        })
        .catch(error => {
            console.error('獲取使用者資訊時發生錯誤:', error);
        })
        .finally(() => {
            // 無論是否成功獲取資訊，都繼續載入留言
            loadMessages();
        });

    document.querySelector('.submit-btn').addEventListener('click', submitMessage);
});

// 提交留言
function submitMessage() {
    const messageInput = document.querySelector('.message-input');
    const content = messageInput.value.trim();

    if (!content) {
        alert('請輸入留言內容！');
        return;
    }

    // 使用 UserName 提交
    fetch('/KoaLaDessertWeb/Contact/AddMessage', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            Role: currentUserName, // 提交 UserName
            MessageContent: content
        })
    })
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP 錯誤！狀態碼: ${response.status}`);
        }
        return response.json();
    })
    .then(data => {
        console.log('AddMessage 回應:', data); // 調試用
        if (data.state === 'Normal' && data.message === 'Success') {
            messageInput.value = ''; // 清空輸入框
            loadMessages(); // 重新載入留言列表
        } else {
            alert('留言提交失敗：' + data.message);
        }
    })
    .catch(error => {
        console.error('提交留言時發生錯誤:', error);
        alert('提交留言時發生錯誤: ' + error.message);
    });
}

// 載入所有留言
function loadMessages() {
    fetch('/KoaLaDessertWeb/Contact/GatMessages')
        .then(response => response.json())
        .then(data => {
            if (data.state === 'Normal' && data.message === 'Success') {
                const messages = data.results;
                const messageList = document.getElementById('messageList');
                messageList.innerHTML = '';

                messages.forEach(message => {
                    const messageElement = document.createElement('div');
                    messageElement.className = 'message-item';
                    // 格式化角色清單
                    const roleDisplay = message.RoleList && message.RoleList.length > 0
                        ? `${message.UserName} (${message.RoleList.join(', ')})`
                        : message.UserName;
                    
                    // 準備留言內容
                    let messageHtml = `
                        <div class="user-id">[${roleDisplay}]</div>
                        <div class="content">${message.MessageContent}</div>
                        <div class="timestamp">[${new Date(message.MessageTime).toLocaleString('zh-TW', { timeZone: 'Asia/Taipei', year: 'numeric', month: 'numeric', day: 'numeric', hour: 'numeric', minute: 'numeric', second: 'numeric', hour12: true })}]</div>
                    `;

                    // 如果當前使用者是 Admin 或 SuperAdmin，添加刪除按鈕
                    if (currentUserRoles.includes('Admin') || currentUserRoles.includes('SuperAdmin')) {
                        messageHtml += `
                            <button class="delete-btn" data-id="${message.Id}">刪除</button>
                        `;
                    }

                    messageElement.innerHTML = messageHtml;
                    messageList.appendChild(messageElement);
                });

                // 為刪除按鈕綁定事件
                document.querySelectorAll('.delete-btn').forEach(button => {
                    button.addEventListener('click', function () {
                        const messageId = parseInt(this.getAttribute('data-id'));
                        deleteMessage(messageId);
                    });
                });
            } else {
                console.error('載入留言失敗:', data.message);
            }
        })
        .catch(error => {
            console.error('載入留言時發生錯誤:', error);
        });
}

// 刪除留言
function deleteMessage(messageId) {
    if (!confirm('確定要刪除這條留言嗎？')) {
        return;
    }

    fetch('/KoaLaDessertWeb/Contact/DeleteMessage', {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(messageId)
    })
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP 錯誤！狀態碼: ${response.status}`);
        }
        return response.json();
    })
    .then(data => {
        console.log('DeleteMessage 回應:', data); // 調試用
        if (data.state === 'Normal' && data.message === 'Success') {
            alert('刪除成功');
            loadMessages(); // 重新載入留言列表
        } else {
            alert('刪除失敗：' + data.result);
        }
    })
    .catch(error => {
        console.error('刪除留言時發生錯誤:', error);
        alert('刪除留言時發生錯誤: ' + error.message);
    });
}

// 支援 Enter 鍵提交
document.querySelector('.message-input').addEventListener('keypress', function (e) {
    if (e.key === 'Enter' && !e.shiftKey) {
        e.preventDefault();
        submitMessage();
    }
});