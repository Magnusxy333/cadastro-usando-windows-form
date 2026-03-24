CREATE DATABASE db_produtos;

USE db_produtos;

CREATE TABLE tb_cadastro_prod (
    id_produto      INT PRIMARY KEY IDENTITY(1,1), -- IDENTITY faz o papel do Auto Increment
    desc_produto    VARCHAR(255) NOT NULL,
    data_lote       DATE,
    categoria_prod  VARCHAR(100),
    marca_produto   VARCHAR(100),
    qnt_dis_prod    INT DEFAULT 0,
    preco_custo     DECIMAL(10, 2) DEFAULT 0.00,
    preco_venda     DECIMAL(10, 2) DEFAULT 0.00,
    foto_prod       VARCHAR(255)
);
 
-- Inserindo peças de carros para teste
INSERT INTO tb_cadastro_prod (desc_produto, data_lote, categoria_prod, marca_produto, qnt_dis_prod, preco_custo, preco_venda, foto_prod)
VALUES
('Amortecedor Dianteiro Direito', GETDATE(), 'Suspensão', 'Cofap', 8, 280.50, 450.00, ''),
('Pastilha de Freio Dianteira', GETDATE(), 'Freio', 'Fras-le', 25, 65.00, 110.00, ''),
('Filtro de Óleo PSL55', GETDATE(), 'Filtros', 'Tecfil', 40, 15.20, 35.00, ''),
('Bateria 60Ah 12V', GETDATE(), 'Elétrica', 'Moura', 12, 320.00, 499.90, ''),
('Correia Dentada Kit', GETDATE(), 'Motor', 'Gates', 10, 145.00, 260.00, ''),
('Lanterna Traseira Lado Esquerdo', GETDATE(), 'Iluminação', 'Honda', 5, 210.00, 385.00, ''),
('Bomba d Água Gol G5', GETDATE(), 'Arrefecimento', 'Urba', 15, 85.40, 165.00, '');
GO
 
-- Verifica se os dados entraram
SELECT * FROM tb_cadastro_prod;
