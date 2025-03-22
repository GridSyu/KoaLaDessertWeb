// 頁面載入時初始化留言列表和事件綁定
document.addEventListener('DOMContentLoaded', function () {
    loadMessages();
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

    const role = '遊客'; // 模擬登入狀態，可根據需求動態調整

    // 發送 POST 請求到 AddMessage API
    fetch('/KoaLaDessertWeb/Contact/AddMessage', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            Role: role,
            MessageContent: content // 改為與後端一致
        })
    })
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP 錯誤！狀態碼: ${response.status}`);
        }
        return response.json();
    })
    .then(data => {
        console.log('回傳資料:', data); // 調試用
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
                    messageElement.innerHTML = `
                        <div class="user-id">[${message.Role}]</div>
                        <div class="content">${message.MessageContent}</div>
                        <div class="timestamp">[${new Date(message.MessageTime).toLocaleString('zh-TW', { timeZone: 'Asia/Taipei' })}]</div>
                    `;
                    messageList.appendChild(messageElement);
                });
            } else {
                console.error('載入留言失敗:', data.message);
            }
        })
        .catch(error => {
            console.error('載入留言時發生錯誤:', error);
        });
}

// 支援 Enter 鍵提交
document.querySelector('.message-input').addEventListener('keypress', function (e) {
    if (e.key === 'Enter' && !e.shiftKey) {
        e.preventDefault();
        submitMessage();
    }
});