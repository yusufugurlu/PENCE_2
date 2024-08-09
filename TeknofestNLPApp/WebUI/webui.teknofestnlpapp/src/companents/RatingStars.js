import React from 'react';

function RatingStars({ rate }) {
    const numberOfStars = rate || 0;
    const stars = [];
    for (let index = 0; index < numberOfStars; index++) {
        stars.push(<i key={index} className="fa fa-star rating-color"></i>);
    }

    return (
        <div className="ratings">
            {stars}
        </div>
    );
}

export default RatingStars;
