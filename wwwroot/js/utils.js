// 工具函式 JS（utils.js）：封裝可重複使用的函數，如格式化時間、表單驗證。


// Email驗證
export function validateEmail(email) {
    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailPattern.test(email);
}