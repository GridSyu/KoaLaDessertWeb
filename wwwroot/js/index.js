import { validateEmail } from "./utils.js";

document.addEventListener("DOMContentLoaded", function () {
    const emailForm = document.getElementById("emailForm");
    const emailInput = document.getElementById("emailInput");
    const resultMessage = document.getElementById("resultMessage");

    if (!emailForm || !emailInput || !resultMessage) {
        console.error("找不到表單或輸入框或結果訊息元素");
        return;
    }

    emailForm.addEventListener("submit", function (event) {
        event.preventDefault(); // 防止表單預設提交

        const email = emailInput.value.trim();

        if (!validateEmail(email)) {
            resultMessage.innerHTML = `<span style="color: red;">❌ 請輸入有效的 Email</span>`;
            return;
        }
        // 調用後端API以方便存入電子信箱
        fetch("/KoaLaDessertWeb/Home/GetUsersEmailAddress", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ email: email })
        })
        .then(response => response.json())
        .then(data => {
            if (data.state === "Success") {
                resultMessage.innerHTML = `<span style="color: green;">✅ ${"回傳狀態：" + data.message}</span>`;
            } else {
                resultMessage.innerHTML = `<span style="color: red;">❌ ${data.message}</span>`;
            }
        })
        .catch(error => {
            console.error("發送請求錯誤:", error);
            resultMessage.innerHTML = `<span style="color: red;">❌ 伺服器錯誤</span>`;
        });
    });
});