// API 基礎路由
// const apiurl = "/KoaLaDessertWeb/Areas/SuperAdmin/Dashboard/";

const apiurl = window.location.pathname.includes("KoaLaDessertWeb")
    ? "/KoaLaDessertWeb/Areas/SuperAdmin/Dashboard/"
    : "/Areas/SuperAdmin/Dashboard/";


$(document).ready(function () {
    // 載入使用者列表
    loadUsers();

    // 儲存角色按鈕事件
    $("#saveRoles").click(saveUserRoles);
});

// 載入使用者列表
function loadUsers() {
    console.log("Loading users...");
    console.log("API URL:", apiurl + "manage-users");
    $.ajax({
        url: apiurl + "manage-users",
        method: "GET",
        success: function (response) {
            const tbody = $("#userTable tbody");
            tbody.empty();
            if (!Array.isArray(response)) {
                console.error("API 回應不是陣列:", response);
                return;
            }
            response.forEach(user => {
                // 使用正確的屬性名：Roles（而不是 roles）
                const roles = Array.isArray(user.Roles) ? user.Roles.join(", ") : "無角色";
                tbody.append(`
                    <tr>
                        <td>${user.Id}</td>    <!-- 使用 Id 而不是 id -->
                        <td>${user.Email}</td> <!-- 使用 Email 而不是 email -->
                        <td>${roles}</td>
                        <td>
                            <button class="btn btn-sm btn-primary edit-roles" data-id="${user.Id}">編輯角色</button>
                        </td>
                    </tr>
                `);
            });

            // 綁定編輯按鈕事件
            $(".edit-roles").click(function () {
                const userId = $(this).data("id");
                loadUserRoles(userId);
            });
        },
        error: function (xhr, status, error) {
            console.error("載入使用者失敗:", error);
            $("#userTable tbody").html(`
                <tr>
                    <td colspan="4" class="text-center text-danger">無法載入使用者列表：${xhr.responseJSON?.message || "未知錯誤"}</td>
                </tr>
            `);
        }
    });
}

// 載入指定使用者的角色資料
function loadUserRoles(userId) {
    $.ajax({
        url: apiurl + `edit-user-roles/${userId}`,
        method: "GET",
        success: function (response) {
            $("#userId").val(response.UserId); // 使用 UserId
            $("#userEmail").text(response.Email); // 使用 Email
            const roleCheckboxes = $("#roleCheckboxes");
            roleCheckboxes.empty();

            response.AllRoles.forEach(role => { // 使用 AllRoles
                const isChecked = response.CurrentRoles.includes(role) ? "checked" : ""; // 使用 CurrentRoles
                roleCheckboxes.append(`
                    <div class="form-check">
                        <input class="form-check-input" type="checkbox" value="${role}" id="role_${role}" ${isChecked}>
                        <label class="form-check-label" for="role_${role}">${role}</label>
                    </div>
                `);
            });

            $("#editRoleModal").modal("show");
        },
        error: function (xhr, status, error) {
            console.error("無法載入角色資料:", error);
        }
    });
}

// 儲存使用者角色
function saveUserRoles() {
    const userId = $("#userId").val();
    const selectedRoles = $("#roleCheckboxes input:checked").map(function () {
        return $(this).val();
    }).get();

    $.ajax({
        url: apiurl + `edit-user-roles/${userId}`,
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify(selectedRoles),
        success: function (response) {
            alert(response.message);
            $("#editRoleModal").modal("hide");
            loadUsers(); // 重新載入使用者列表
        },
        error: function (xhr, status, error) {
            console.error("儲存失敗:", error);
            alert("儲存失敗：" + (xhr.responseJSON?.message || "未知錯誤"));
        }
    });
}