// 載入商品資料
$(document).ready(function () {
    // 載入標籤和商品
    loadTags();
    loadProducts();

    // 點擊分類時觸發篩選
    $(document).on('click', '.category-link', function (e) {
        e.preventDefault();
        $('.category-link').removeClass('active');
        $(this).addClass('active');
        const tag = $(this).data('tag');
        loadProducts(tag);
    });
});

// API 基礎路由
const apiurl = "http://localhost:5198/KoaLaDessertWeb/Products/";

// 載入商品標籤(左側選單)
function loadTags() {
    $.ajax({
        url: apiurl + 'GetTags',
        method: 'GET',
        success: function (response) {
            if (response.state === 'Normal') {
                const tags = response.results;
                const categoryList = $('#categoryList');
                // 只清空動態生成的標籤，保留第一個「所有商品」
                categoryList.find('li:not(:first)').remove();
                tags.forEach(tag => {
                    const listItem = $('<li>').addClass('category-item');
                    const link = $('<a>').addClass('category-link')
                                        .attr('href', '#')
                                        .attr('data-tag', tag.Name)
                                        .text(tag.Name);
                    listItem.append(link);
                    categoryList.append(listItem);
                });
            } else {
                console.error('載入標籤失敗:', response.message);
            }
        },
        error: function (xhr, status, error) {
            console.error('Error loading tags:', error);
        }
    });
}
// 點擊標籤懸停效果
$(document).on('click', '.category-link', function (e) {
    e.preventDefault();
    $('.category-link').removeClass('active');
    $(this).addClass('active');
    const tag = $(this).data('tag');
    loadProducts(tag);
});

// 讀取商品卡片
function loadProducts(tag = null) {
    const url = tag ? `${apiurl}GetProducts?tag=${encodeURIComponent(tag)}` : apiurl + 'GetProducts';
    $.ajax({
        url: url,
        method: 'GET',
        success: function (response) {
            console.log('Products Response:', response);
            if (response.state === 'Normal') {
                const products = response.results;
                const container = $('#productContainer');
                container.empty();
                products.forEach(product => {
                    container.append(`
                        <div class="col-md-5 mb-4">
                            <div class="card">
                                <img src="${product.ImageUrl}" class="card-img-top" alt="${product.Name}">
                                <div class="card-body">
                                    <h5 class="card-title">${product.Name}</h5>
                                    <p class="card-text">
                                        <strong>價格:</strong> $${product.Price}<br>
                                        <strong>規格:</strong> ${product.Specs}<br>
                                        <strong>介紹:</strong> ${product.Description}
                                    </p>
                                    <a href="#" class="btn btn-primary">加入購物車</a>
                                </div>
                            </div>
                        </div>
                    `);
                });
            } else {
                console.error('Failed to load products:', response.message);
            }
        },
        error: function (xhr, status, error) {
            console.error('Error loading products:', error);
        }
    });
}