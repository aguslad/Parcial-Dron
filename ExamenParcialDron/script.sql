-- 1. Creación de la tabla principal (Cabecera)
CREATE TABLE tb_master_control (
    id SERIAL PRIMARY KEY,
    fecha_sistema TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,
    terreno_n INT NOT NULL,
    despegue_x INT NOT NULL,
    despegue_y INT NOT NULL
);

-- 2. Creación de la tabla de detalle
CREATE TABLE tb_det_log (
    id SERIAL PRIMARY KEY,
    master_id INT NOT NULL,
    paso_etiqueta INT NOT NULL,
    posicion_x INT NOT NULL,
    posicion_y INT NOT NULL,
    CONSTRAINT fk_det_master FOREIGN KEY (master_id) REFERENCES tb_master_control(id) ON DELETE CASCADE
);