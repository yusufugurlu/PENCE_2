import React, { useEffect, useState } from 'react';
import commentService from '../services/commentService';
import AlertService from '../toast/AlertService';
import { Spinner } from 'react-bootstrap';
import CustomComment from './CustomComment';
import { LabelStatus } from '../common/LabelStatus';


function Comments() {
    const [data, setData] = useState([]);
    const [url, setUrl] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [totalElements, setTotalElements] = useState('0');
    const [currentPage, setCurrentPage] = useState(-1);
    const [totalUsefullComment, setTotalUsefullComment] = useState(0);
    const [totalUnUsefullComment, setTotalUnUsefullComment] = useState(0);
    const [currentStatus, setCurrentStatus] = useState(-1);
    const [comments, setComments] = useState([]);

    const handleInputChange = (e) => {
        setUrl(e.target.value);
    };

    useEffect(() => {
        if (currentPage !== -1) {
            fetchDataAll(-1);
        }
    }, [currentPage]);


    const fetchDataAll = async (pageNumber) => {
        try {
            let data = {
                url: url,
                page: pageNumber
            };
            setIsLoading(true);
            setData([]);
            setTotalElements("0");
            const result = await commentService.getData(data);
            setIsLoading(false);
            if (result.status === 200) {
                console.log(result.data);
                setData(result.data.data);
                setComments(result.data.data);
                setTotalElements(result.data.totalElements);
                setTotalUsefullComment(result.data.totalUsefullComment);
                setTotalUnUsefullComment(result.data.totalUnUsefullComment);
            }
            else {
                AlertService.showAlert('Hata!', 'Veri gönderilemedi', 'error');
            }
            //setData(result);
        } catch (error) {
            AlertService.showAlert('Hata!', 'Veri gönderilemedi', 'error');
        }
    }


    const handleButtonClick = () => {
        setCurrentPage(1);
        fetchDataAll(1, true);
    };


    const handleChangeStatus = (event) => {
        const value = event.target.value;
        setCurrentStatus(value);
        
        if(event.target.value == -1){
            setData(comments);
        }
        else if(event.target.value == 0){
            const usefullComments = comments.filter(comment => comment.label === LabelStatus.UseFull);
            setData(usefullComments);
        }
        else if(event.target.value == 1){
            const unUsefullComments = comments.filter(comment => comment.label === LabelStatus.UnUsefull);
            setData(unUsefullComments);
        }
    };

    return (
        <>
            <div class="container d-flex justify-content-center mt-100 mb-100">
                <div class="row">
                    <div class="col-md-12">
                        <div class="card  mb-5">
                            <div class="card-body" style={{ minWidth: '600px' }}>
                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="input-group">
                                            <input type="text" class="form-control" placeholder="Trendyol Ürün URL'yi giriniz..."
                                                value={url}
                                                onChange={handleInputChange} // Input değişikliklerini takip etmek için
                                            />
                                            <div class="input-group-append">
                                                <button class="btn btn-outline-secondary" type="button"
                                                    onClick={handleButtonClick} // Butona tıklama olayını dinlemek için
                                                    disabled={isLoading}
                                                    variant="primary"
                                                >
                                                    {
                                                        isLoading ? (<>
                                                            <Spinner
                                                                as="span"
                                                                animation="grow"
                                                                size="sm"
                                                                role="status"
                                                                aria-hidden="true"
                                                            />
                                                            Yükleniyor...
                                                        </>) : <>
                                                            Yorumları Getir
                                                        </>
                                                    }
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        {
                            data && data.length > 0 ? <>
                                <div class="card mb-3">
                                    <div class="card-body">
                                        <h4 class="card-title">Yorumlar Bölümü</h4>
                                        <div class="card-subtitle">
                                            <div>
                                                Toplam <strong>{totalElements}</strong> yorum bulundu.
                                            </div>
                                            <div>
                                                Toplam <strong>{totalUsefullComment}</strong> faydalı yorum bulundu.
                                            </div>
                                            <div>
                                                Toplam <strong>{totalUnUsefullComment}</strong> faydalsız yorum bulundu.
                                            </div>

                                        </div>

                                        <div className="form-group">

                                            <label htmlFor="status">Filtre</label>
                                            <select id="status"  className="form-control" value={currentStatus} onChange={handleChangeStatus}>
                                                <option value={-1}>Tümü</option>
                                                <option value={LabelStatus.UseFull}>{"Faydalı Yorumlar"}</option>
                                                <option value={LabelStatus.UnUsefull}>{"Faydasız Yorumlar"}</option>
                                            </select>
                                        </div>
                                    </div>
                                </div>
                            </> : null
                        }
                        {
                            currentPage !== -1 ? <>
                                <div class="card">
                                    <div class="comment-widgets m-b-20">
                                        {!isLoading ? (
                                            <>
                                                {
                                                    data.map((comment, index) => (
                                                        <CustomComment index={index} comment={comment} />
                                                    ))
                                                }
                                            </>
                                        ) : (
                                            <>
                                                <div className='row m-5'>
                                                    <div className='col-md-12'>
                                                        <Spinner
                                                            as="span"
                                                            animation="grow"
                                                            size="sm"
                                                            role="status"
                                                            aria-hidden="true"
                                                        />
                                                        Yükleniyor...
                                                    </div>
                                                </div>
                                            </>
                                        )}
                                    </div>
                                </div>
                            </> : <></>
                        }

                    </div>
                </div>
            </div >
        </>
    );
}

export default Comments;
