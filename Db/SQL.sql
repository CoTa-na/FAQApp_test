-- =========================================
-- 1. テーブル作成: faqs
-- =========================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'faqs')
BEGIN
    CREATE TABLE faqs (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Title NVARCHAR(200) NOT NULL,         -- FAQタイトル
        Content NVARCHAR(MAX) NOT NULL,       -- FAQ本文（Markdown想定）
        CreatedBy NVARCHAR(100) NOT NULL,     -- 作成者（簡易ログイン名などを想定）
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(), -- 作成日時
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE()  -- 更新日時
    );
    PRINT 'Table faqs created.';
END
ELSE
BEGIN
    PRINT 'Table faqs already exists.';
END
GO

-- =========================================
-- 2. ストアド: 一覧取得 兼 検索 (SELECT)
-- =========================================
CREATE OR ALTER PROCEDURE usp_GetFaqList
    @SearchKeyword NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- キーワードが空文字の場合はNULLとして扱う
    IF LTRIM(RTRIM(@SearchKeyword)) = '' SET @SearchKeyword = NULL;

    SELECT 
        Id, 
        Title, 
        CreatedBy, 
        CreatedAt, 
        UpdatedAt
    FROM 
        faqs
    WHERE 
        (@SearchKeyword IS NULL 
         OR Title LIKE '%' + @SearchKeyword + '%' 
         OR Content LIKE '%' + @SearchKeyword + '%')
    ORDER BY 
        CreatedAt DESC; -- 最新順に表示
END
GO

-- =========================================
-- 3. ストアド: 詳細取得 (SELECT) ※編集画面用
-- =========================================
CREATE OR ALTER PROCEDURE usp_GetFaqDetail
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id, 
        Title, 
        Content, 
        CreatedBy, 
        CreatedAt, 
        UpdatedAt
    FROM 
        faqs
    WHERE 
        Id = @Id;
END
GO

-- =========================================
-- 4. ストアド: 新規登録 (INSERT)
-- =========================================
CREATE OR ALTER PROCEDURE usp_CreateFaq
    @Title NVARCHAR(200),
    @Content NVARCHAR(MAX),
    @CreatedBy NVARCHAR(100),
    @NewId INT OUTPUT -- 登録されたIDを返す用
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO faqs (Title, Content, CreatedBy, CreatedAt, UpdatedAt)
    VALUES (@Title, @Content, @CreatedBy, GETDATE(), GETDATE());

    -- 採番されたIDを取得
    SET @NewId = SCOPE_IDENTITY();
END
GO

-- =========================================
-- 5. ストアド: 更新 (UPDATE)
-- =========================================
CREATE OR ALTER PROCEDURE usp_UpdateFaq
    @Id INT,
    @Title NVARCHAR(200),
    @Content NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE faqs
    SET 
        Title = @Title,
        Content = @Content,
        UpdatedAt = GETDATE()
    WHERE 
        Id = @Id;
END
GO

-- =========================================
-- 6. ストアド: 削除 (DELETE)
-- =========================================
CREATE OR ALTER PROCEDURE usp_DeleteFaq
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM faqs
    WHERE Id = @Id;
END
GO

PRINT 'All stored procedures created successfully.';
GO